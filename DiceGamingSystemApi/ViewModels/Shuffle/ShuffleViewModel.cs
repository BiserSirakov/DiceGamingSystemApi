using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DiceGamingSystemApi.ViewModels.Shuffle
{
    public class ShuffleViewModel
    {
        [Required]
        [Range(1, 12, ErrorMessage = "The bet must be between 1 and 12.")]
        public int Bet { get; set; }

        [Required]
        public int Stake { get; set; }
    }
}