using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DiceGamingSystemApi.Models
{
    public class Currency
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [Range(0, 1000)]
        public int Value { get; set; }

        [Required]
        public string UserId { get; set; }

        public virtual User User { get; set; }
    }
}