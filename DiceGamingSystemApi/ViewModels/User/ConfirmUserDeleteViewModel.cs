using System.ComponentModel.DataAnnotations;

namespace DiceGamingSystemApi.ViewModels.User
{
    public class ConfirmUserDeleteViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}