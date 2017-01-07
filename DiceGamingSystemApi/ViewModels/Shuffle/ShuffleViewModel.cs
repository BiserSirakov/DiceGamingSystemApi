using System.ComponentModel.DataAnnotations;

namespace DiceGamingSystemApi.ViewModels.Shuffle
{
    public class ShuffleViewModel
    {
        [Required]
        [Range(2, 12, ErrorMessage = "The bet must be between 2 and 12.")]
        public int Bet { get; set; }

        [Required]
        public decimal Stake { get; set; }
    }
}