using System.ComponentModel.DataAnnotations;

namespace DiceGamingSystemApi.ViewModels.User
{
    public class UserInfoViewModel
    {
        public string FullName { get; set; }

        public string Username { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}