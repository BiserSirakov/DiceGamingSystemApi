using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DiceGamingSystemApi.ViewModels.Shuffle
{
    public class ShuffleSingleViewModel
    {
        public DateTime Timestamp { get; set; }

        public int Stake { get; set; }

        public int Win { get; set; }

        public int Bet { get; set; }

        public int Result { get; set; }
    }
}