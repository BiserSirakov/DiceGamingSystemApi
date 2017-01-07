using System.Data.Entity;

using Microsoft.AspNet.Identity.EntityFramework;

namespace DiceGamingSystemApi.Models
{
    public class DiceGamingSystemApiDbContext : IdentityDbContext<User>
    {
        public DiceGamingSystemApiDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        
        public IDbSet<Shuffle> Shuffles { get; set; }

        public static DiceGamingSystemApiDbContext Create()
        {
            return new DiceGamingSystemApiDbContext();
        }
    }
}