using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SC2Balance.ViewModels
{
    public class RaceWinRate
    {
        public float TVPWinRate { get; set; }
        public float TVZWinRate { get; set; }
        public float ZVPWinRate { get; set; }
        
        public bool IsEmpty()
        {
            return TVPWinRate.Equals(0) && TVZWinRate.Equals(0) && ZVPWinRate.Equals(0);
        }
   
    }
}