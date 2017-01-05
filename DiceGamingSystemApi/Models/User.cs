﻿using System;
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
        private ICollection<Shuffle> shuffles;

        public User()
        {
            this.currencies = new HashSet<Currency>();
            this.shuffles = new HashSet<Shuffle>();
        }

        [Required]
        [MaxLength(20)]
        public string FullName { get; set; }

        public virtual ICollection<Currency> Currencies
        {
            get { return this.currencies; }
            set { this.currencies = value; }
        }

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