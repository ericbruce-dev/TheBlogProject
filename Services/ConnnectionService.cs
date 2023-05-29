using Npgsql;

namespace TheBlogProject.Services
{
    public class ConnnectionService
    {
        public static class ConnectionHelper
        {
            public static string GetConnectionString(IConfiguration configuration)
            {
                //var connectionString = configuration.GetConnectionString("DefaultConnection");
                //var connectionString = configuration.GetConnectionString("Server=localhost;Port=5432;Database=TheBlogProject;User Id=postgres;Password=3agl35;Trusted_Connection=True;MultipleActiveResultSets=True;");
                var connectionString = configuration.GetConnectionString("Server=postgresql://postgres:VgaUSimVSyrRVoENfA1x@containers-us-west-161.railway.app:5745/railway;Port=5432;Database=railway;User Id=postgres;Password=VgaUSimVSyrRVoENfA1x;Trusted_Connection=True;MultipleActiveResultSets=True;");
                var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
                return string.IsNullOrEmpty(databaseUrl) ? connectionString : BuildConnectionString(databaseUrl);
            }

            //build the connection string from the environment. i.e. Heroku
            private static string BuildConnectionString(string databaseUrl)
            {
                var databaseUri = new Uri(databaseUrl);
                var userInfo = databaseUri.UserInfo.Split(':');
                var builder = new NpgsqlConnectionStringBuilder
                {
                    Host = databaseUri.Host,
                    Port = databaseUri.Port,
                    Username = userInfo[0],
                    Password = userInfo[1],
                    Database = databaseUri.LocalPath.TrimStart('/'),
                    SslMode = SslMode.Require,
                    TrustServerCertificate = true
                };
                return builder.ToString();
            }
        }
    }
}
