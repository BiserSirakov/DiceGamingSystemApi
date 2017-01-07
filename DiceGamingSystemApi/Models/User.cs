using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DiceGamingSystemApi.Models
{
    public class User : IdentityUser
    {
        private ICollection<Shuffle> shuffles;

        public User()
        {
            this.shuffles = new HashSet<Shuffle>();
        }

        [MaxLength(20)]
        public string FullName { get; set; }

        public decimal VirtualMoney { get; set; }

        public virtual ICollection<Shuffle> Shuffles
        {
            get { return this.shuffles; }
            set { this.shuffles = value; }
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager, string authenticationType)
        {
            return await manager.CreateIdentityAsync(this, authenticationType);
        }
    }
}