using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SC2Balance.Models
{
    public class MapWinRate
    {
        public string Map { get; set; }
        public WinRate WinRate { get; set; }
    }
}