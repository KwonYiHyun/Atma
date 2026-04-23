using Dapper;
using Microsoft.Data.SqlClient;

namespace LoginServer
{
    public class DatabaseInitializer
    {
        public static void Initialize(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            var targetDatabase = builder.InitialCatalog;

            builder.InitialCatalog = "master";

            using (var masterConnection = new SqlConnection(builder.ConnectionString))
            {
                masterConnection.Open();

                var checkDbSql = $"SELECT 1 FROM sys.databases WHERE name = N'{targetDatabase}'";
                var dbExists = masterConnection.ExecuteScalar<int>(checkDbSql) == 1;

                if (!dbExists)
                {
                    masterConnection.Execute($"CREATE DATABASE [{targetDatabase}]");
                }
            }

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var createTableSql = @"
                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='person' and xtype='U')
                    BEGIN
                        CREATE TABLE person (
                            person_id INT IDENTITY(1,1) PRIMARY KEY,
                            display_person_id INT NOT NULL CONSTRAINT UQ_DisplayPersonId UNIQUE,
                            login_provider NVARCHAR(50) NOT NULL,
                            person_hash NVARCHAR(255) NOT NULL,
                            email NVARCHAR(255),
                            insert_date DATETIME DEFAULT GETDATE(),
                            update_date DATETIME DEFAULT GETDATE()
                        );

                        CREATE NONCLUSTERED INDEX IX_Person_LoginProvider_Hash 
                        ON person (login_provider, person_hash);
                    END";

                connection.Execute(createTableSql);
            }
        }
    }
}
