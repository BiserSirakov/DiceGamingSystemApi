namespace DiceGamingSystemApi.Config
{
    using System.Data.Entity;
    using Migrations;
    using Models;

    public class DatabaseConfig
    {
        public static void Initialize()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>());
            ApplicationDbContext.Create().Database.Initialize(true);
        }
    }
}