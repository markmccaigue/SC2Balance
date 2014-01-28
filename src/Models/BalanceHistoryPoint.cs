using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SC2Balance.Models
{
    public class BalanceHistoryPoint
    {
        public DateTime DateTime { get; set; }
        public WinRate WinRate { get; set; }
    }
}