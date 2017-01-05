using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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