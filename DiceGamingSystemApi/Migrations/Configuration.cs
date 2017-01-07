using System.Data.Entity.Migrations;

using DiceGamingSystemApi.Models;

namespace DiceGamingSystemApi.Migrations
{
    public sealed class Configuration : DbMigrationsConfiguration<DiceGamingSystemApiDbContext>
    {
        public Configuration()
        {
            this.AutomaticMigrationsEnabled = true;
            this.AutomaticMigrationDataLossAllowed = true;
        }
    }
}