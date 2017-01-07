using System;

namespace DiceGamingSystemApi.ViewModels.Shuffle
{
    public class ShufflesListViewModel
    {
        public DateTime Timestamp { get; set; }

        public decimal Stake { get; set; }

        public decimal Win { get; set; }
    }
}