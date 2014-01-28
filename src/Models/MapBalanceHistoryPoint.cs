using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SC2Balance.Models
{
    public class MapBalanceHistoryPoints
    {
        public ICollection<BalanceHistoryPoint> BalanceHistoryPoints { get; set; }
        public String Map { get; set; }
    }
}