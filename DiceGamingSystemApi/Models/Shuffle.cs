using System;
using System.ComponentModel.DataAnnotations;

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
        public decimal Stake { get; set; }

        public int Result { get; set; }

        public decimal Win { get; set; }

        public DateTime Timestamp { get; set; }
    }
}