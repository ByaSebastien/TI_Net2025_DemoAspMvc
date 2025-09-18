using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TI_Net2025_DemoAspMvc.Mappers;
using TI_Net2025_DemoAspMvc.Models.Dtos.Author;
using TI_Net2025_DemoAspMvc.Models.Dtos.Book;
using TI_Net2025_DemoAspMvc.Models.Entities;
using TI_Net2025_DemoAspMvc.Repositories;

namespace TI_Net2025_DemoAspMvc.Controllers
{
    public class BookController : Controller
    {

        private readonly BookRepository _bookRepository;
        private readonly AuthorRepository _authorRepository;

        public BookController(BookRepository bookRepository, AuthorRepository authorRepository)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
        }

        public IActionResult Index()
        {
            List<Book> books = _bookRepository.GetAllBooksWithAuthor();

            List<BookIndexDto> dtos = books
                .Select(book => book.ToBookIndexDto())
                .ToList();

            return View(dtos);
        }

        [HttpGet("/book/details/{isbn}")]
        public IActionResult Details([FromRoute] string isbn)
        {
            Book? book = _bookRepository.GetOneWithAuthor(isbn);

            if(book == null)
            {
                throw new Exception($"Book with isbn {isbn} not found");
            }

            return View(book.ToBookDetailDto());
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Add()
        {
            List<Author> authors = _authorRepository.GetAll();

            List<AuthorDto> authorDtos = authors
                .Select(a => a.ToAuthorDto())
                .ToList();

            ViewData.Add("Authors", authorDtos);

            return View(new BookFormDto());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Add([FromForm] BookFormDto book)
        {

            if (!ModelState.IsValid)
            {
                List<Author> authors = _authorRepository.GetAll();

                List<AuthorDto> authorDtos = authors
                    .Select(a => a.ToAuthorDto())
                    .ToList();

                ViewData.Add("Authors", authorDtos);

                return View(book);
            }

            if (_bookRepository.ExistById(book.Isbn))
            {
                throw new Exception($"Book with isbn {book.Isbn} already exist");
            }

            if (!_authorRepository.ExistById(book.AuthorId))
            {
                throw new Exception($"Author with id {book.AuthorId} doesn't exist");
            }

            _bookRepository.Add(book.ToBook());

            return RedirectToAction("Index", "Book");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("/book/edit/{isbn}")]
        public IActionResult Edit([FromRoute] string isbn)
        {
            Book? book = _bookRepository.GetById(isbn);

            if (book == null)
            {
                throw new Exception($"Book with isbn {isbn} not found");
            }

            List<Author> authors = _authorRepository.GetAll();

            List<AuthorDto> authorDtos = authors
                .Select(a => a.ToAuthorDto())
                .ToList();

            ViewData.Add("Authors", authorDtos);

            return View(book.ToBookFormDto());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("/book/edit/{isbn}")]
        public IActionResult Edit([FromRoute] string isbn, [FromForm] BookFormDto book)
        {
            if (!ModelState.IsValid)
            {
                List<Author> authors = _authorRepository.GetAll();

                List<AuthorDto> authorDtos = authors
                    .Select(a => a.ToAuthorDto())
                    .ToList();

                ViewData.Add("Authors", authorDtos);

                return View(book);
            }

            if (!_bookRepository.ExistById(isbn))
            {
                throw new Exception($"Book with isbn {isbn} doesn't exist");
            }

            if (!_authorRepository.ExistById(book.AuthorId))
            {
                throw new Exception($"Author with id {book.AuthorId} doesn't exist");
            }

            if ( isbn != book.Isbn && _bookRepository.ExistById(book.Isbn))
            {
                throw new Exception($"Book with isbn {book.Isbn} already exist");
            }

            _bookRepository.Update(isbn, book.ToBook());

            return RedirectToAction("Index", "Book");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("/book/remove/{isbn}")]
        public IActionResult Remove([FromRoute] string isbn)
        {

            if (!_bookRepository.ExistById(isbn))
            {
                throw new Exception($"Book with isbn {isbn} doesn't exist");
            }

            _bookRepository.DeleteById(isbn);

            return RedirectToAction("Index", "Book");
        }
    }
}
