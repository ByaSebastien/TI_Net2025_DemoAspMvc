using Isopoh.Cryptography.Argon2;
using TI_Net2025_DemoAspMvc.Models;
using TI_Net2025_DemoAspMvc.Models.Dtos.User;
using TI_Net2025_DemoAspMvc.Models.Entities;
using TI_Net2025_DemoAspMvc.Repositories;

namespace TI_Net2025_DemoAspMvc.Services
{
    public class UserService
    {

        private readonly UserRepository _userRepository;

        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public void Add(User user)
        {
            if (_userRepository.ExistByEmail(user.Email))
            {
                throw new Exception($"User with email {user.Email} already exist");
            }

            if (_userRepository.ExistByUsername(user.Username))
            {
                throw new Exception($"User with username {user.Username} already exist");
            }

            user.Password = Argon2.Hash(user.Password);
            user.Role = UserRole.User;

            _userRepository.Add(user);
        }
    }
}
