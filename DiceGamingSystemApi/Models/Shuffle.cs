using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DiceGamingSystemApi.Models
{
    public class Shuffle
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public virtual User User { get; set; }

        [Required]
        public int Bet { get; set; }

        [Required]
        public int Stake { get; set; }

        public int Result { get; set; }

        public int Win { get; set; }

        public DateTime Timestamp { get; set; }
    }
}