using System.Data.SqlClient;
using System.Diagnostics;

namespace LogstashConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            DeleteDocument();
            DeleteElastic();
        }

        private static void DeleteDocument()
        {
            string connectionString = GetConnectionString();

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = connectionString;

                connection.Open();
                SqlCommand command = new SqlCommand("BEGIN TRAN SELECT 1 FROM [user] WITH (TABLOCKX); WAITFOR DELAY '00:00:5'; delete from [user] where [is_deleted]=1; ROLLBACK TRAN;", connection);
                command.ExecuteNonQuery();
                command = new SqlCommand("delete from [user] where [is_deleted]=1", connection);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        static private string GetConnectionString()
        {
            // To avoid storing the connection string in your code, 
            // you can retrieve it from a configuration file.
            return "Data Source=DESKTOP-GT4AAMB;Initial Catalog=mssql_server;"
                + "Integrated Security=SSPI";
        }

        private static void DeleteElastic()
        {
            var process = new Process();
            process.StartInfo.FileName = "curl";
            process.StartInfo.Arguments = "-XPOST http://localhost:9200/starindex01/_doc/_delete_by_query?q=is_deleted:1";
            process.Start();
            process.Close();
        }
    }
}
