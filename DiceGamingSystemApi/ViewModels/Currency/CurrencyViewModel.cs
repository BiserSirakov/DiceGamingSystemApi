using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DiceGamingSystemApi.ViewModels.Currency
{
    public class CurrencyViewModel
    {
        [Required]
        [Range(0, 1000, ErrorMessage = "The value must be between 0 and 1000.")]
        public int Value { get; set; }
    }
}