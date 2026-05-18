using Dapper;
using LoginServer.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace LoginServer.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly string _connectionString;

        public PersonRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AccountConnection");
        }

        private IDbConnection createConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public async Task<int> createPersonAsync(person person, IDbTransaction? transaction = null)
        {
            var sql = @"
                INSERT INTO person (display_person_id, login_provider, person_hash, email, insert_date, update_date)
                VALUES (@display_person_id, @login_provider, @person_hash, @email, @insert_date, @update_date);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            var conn = transaction?.Connection ?? createConnection();

            try
            {
                return await conn.ExecuteScalarAsync<int>(sql, person, transaction);
            }
            finally
            {
                if (transaction == null)
                {
                    conn.Dispose();
                }
            }
        }

        public async Task<person?> getPersonByPersonIdAsync(int personId)
        {
            var sql = "SELECT * FROM person WHERE person_id = @person_id";

            using (var db = createConnection())
            {
                return await db.QuerySingleOrDefaultAsync<person>(sql, new { person_id = personId });
            }
        }

        public async Task<person?> getPersonByDisplayPersonIdAsync(string provider, int displayPersonId)
        {
            var sql = "SELECT * FROM person WHERE display_person_id = @display_person_id AND login_provider = @login_provider";

            using (var db = createConnection())
            {
                return await db.QuerySingleOrDefaultAsync<person>(sql, new { login_provider = provider, display_person_id = displayPersonId });
            }
        }

        public async Task<person?> getPersonByHashAsync(string provider, string hash)
        {
            var sql = "SELECT * FROM person WHERE person_hash = @person_hash AND login_provider = @login_provider";

            using (var db = createConnection())
            {
                return await db.QuerySingleOrDefaultAsync<person>(sql, new { login_provider = provider, person_hash = hash });
            }
        }

        public async Task<bool> existsDisplayIdAsync(int displayPersonId)
        {
            var sql = "SELECT COUNT(1) FROM person WHERE display_person_id = @display_person_id";
            using (var db = createConnection())
            {
                int count = await db.ExecuteScalarAsync<int>(sql, new { display_person_id = displayPersonId });
                return count > 0;
            }
        }

        public async Task<bool> deletePersonAsync(int personId)
        {
            var sql = "DELETE FROM person WHERE person_id = @person_id";

            using (var db = createConnection())
            {
                int affectedRows = await db.ExecuteAsync(sql, new { person_id = personId });

                return affectedRows > 0;
            }
        }
    }
}
