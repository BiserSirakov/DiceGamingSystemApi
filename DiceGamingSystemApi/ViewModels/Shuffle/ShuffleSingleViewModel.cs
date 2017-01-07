using System;

namespace DiceGamingSystemApi.ViewModels.Shuffle
{
    public class ShuffleSingleViewModel
    {
        public DateTime Timestamp { get; set; }

        public decimal Stake { get; set; }

        public decimal Win { get; set; }

        public int Bet { get; set; }

        public int Result { get; set; }
    }
}