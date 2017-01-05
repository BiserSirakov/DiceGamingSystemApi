namespace DiceGamingSystemApi.Config
{
    using System.Data.Entity;
    using Migrations;
    using Models;

    public class DatabaseConfig
    {
        public static void Initialize()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DiceGamingSystemApiDbContext, Configuration>());
            DiceGamingSystemApiDbContext.Create().Database.Initialize(true);
        }
    }
}