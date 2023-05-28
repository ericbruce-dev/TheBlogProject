using Microsoft.EntityFrameworkCore;
using TheBlogProject.Data;

namespace TheBlogProject.Services
{
    public class MigrationDataHelper
    {
        public static class DataHelper
        {

            public static async Task ManageDataAsync(IServiceProvider svcProvider)
            {
                //Service: An instance of db context
                var dbContextSvc = svcProvider.GetRequiredService<ApplicationDbContext>();

                //Migration: This is the programmatic equivalent to Update-Database
                await dbContextSvc.Database.MigrateAsync();
            }


        }
    }
}
