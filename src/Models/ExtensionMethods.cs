using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SC2Balance.Models
{
    public static class ExtensionMethods
    {
        public static string RegionCode(this LadderRegion region)
        {
            switch (region)
            {
                case LadderRegion.Europe:
                    return "eu";
                case LadderRegion.Korea:
                    return "kr";
                case LadderRegion.NorthAmerica:
                    return "us";
                default:
                    return region.ToString();
            }
        }

    }
}
