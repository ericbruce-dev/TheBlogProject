﻿using Npgsql;

namespace TheBlogProject.Services
{
         public static class ConnectionHelper
        {
            public static string GetConnectionString(IConfiguration configuration)
            {
                //var connectionString = configuration.GetSection("pgSettings")["pgConnection"];
                string databaseUrl = "postgresql:///postgres:VgaUSimVSyrRVoENfA1x@containers-us-west-161.railway.app:5745//railway";
                return BuildConnectionString(databaseUrl);
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

