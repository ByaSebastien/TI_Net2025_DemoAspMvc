using Microsoft.Data.SqlClient;
using TI_Net2025_DemoAspMvc.Models.Entities;

namespace TI_Net2025_DemoAspMvc.Repositories
{
    public class AuthorRepository: BaseRepository<Author,int>
    {
        protected override string TableName => "AUTHOR";

        protected override string ColumnIdName => "ID";

        public override void Add(Author entity)
        {
            throw new NotImplementedException();
        }

        public override void Update(int id, Author entity)
        {
            throw new NotImplementedException();
        }

        public override Author MapEntity(SqlDataReader reader)
        {
            return new Author()
            {
                Id = (int)reader["ID"],
                Firstname = (string)reader["FIRST_NAME"],
                Lastname = (string)reader["LAST_NAME"],
            };
        }
    }
}
