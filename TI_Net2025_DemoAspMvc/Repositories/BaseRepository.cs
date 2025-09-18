using Microsoft.Data.SqlClient;
using System.Data;

namespace TI_Net2025_DemoAspMvc.Repositories
{
    public abstract class BaseRepository<TEntity, TId> where TEntity : class
    {
        protected readonly string _connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=BookDb;Trusted_Connection=True;";
        protected abstract string TableName { get; }
        protected abstract string ColumnIdName { get; }


        public List<TEntity> GetAll()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = $@"SELECT * FROM {TableName};";

                OpenConnection(connection);

                using(SqlDataReader reader = command.ExecuteReader())
                {
                    List<TEntity> entities = [];
                    while (reader.Read())
                    {
                        entities.Add(MapEntity(reader));
                    }
                    return entities;
                }
            }
        }

        public TEntity? GetById(TId id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = $@"SELECT * 
                                         FROM {TableName}
                                         WHERE {ColumnIdName} = @id;";

                command.Parameters.AddWithValue("@id", id);

                OpenConnection(connection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }
                    return MapEntity(reader);
                }
            }
        }

        public bool ExistById(TId id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @$"SELECT 
                                         cast(CASE WHEN EXISTS (
                                            SELECT 1 FROM {TableName} WHERE {ColumnIdName} = @id) 
                                             THEN 1 
                                             ELSE 0 
                                         END as bit) AS isExisting;";

                command.Parameters.AddWithValue("@id", id);

                OpenConnection(connection);

                return (bool)command.ExecuteScalar();
            }
        }

        public bool DeleteById(TId id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @$"DELETE FROM {TableName}
                                         WHERE {ColumnIdName} = @id";

                command.Parameters.AddWithValue("@id", id);

                OpenConnection(connection);

                return (bool)command.ExecuteScalar();
            }
        }

        public abstract void Add(TEntity entity);
        public abstract void Update(TId id, TEntity entity);

        public abstract TEntity MapEntity(SqlDataReader reader);

        public void OpenConnection(SqlConnection connection)
        {
            if(connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
        }
    }
}
