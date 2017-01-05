using System.ComponentModel.DataAnnotations;

namespace DiceGamingSystemApi.ViewModels.User
{
    public class ChangeUserInfoViewModel
    {
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string FullName { get; set; }
    }
}