using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;

namespace DiceGamingSystemApi.Models
{
    public class User : IdentityUser
    {
        private ICollection<Currency> currencies;

        public User()
        {
            this.currencies = new HashSet<Currency>();
        }

        [Required]
        [MaxLength(20)]
        public string FullName { get; set; }

        public virtual ICollection<Currency> Currencies
        {
            get { return this.currencies; }
            set { this.currencies = value; }
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager, string authenticationType)
        {
            return await manager.CreateIdentityAsync(this, authenticationType);
        }
    }
}