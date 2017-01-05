using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace DiceGamingSystemApi.Models
{
    public class DiceGamingSystemApiDbContext : IdentityDbContext<User>
    {
        public DiceGamingSystemApiDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public IDbSet<Currency> Currencies { get; set; }

        public IDbSet<Shuffle> Shuffles { get; set; }

        public static DiceGamingSystemApiDbContext Create()
        {
            return new DiceGamingSystemApiDbContext();
        }
    }
}