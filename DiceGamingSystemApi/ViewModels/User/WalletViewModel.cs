using System.ComponentModel.DataAnnotations;

namespace DiceGamingSystemApi.ViewModels.User
{
    public class WalletViewModel
    {
        [Required]
        [Range(0, 1000, ErrorMessage = "The amount of virtual money you can add must be from 0 to 1000.")]
        public decimal Amount { get; set; }
    }
}