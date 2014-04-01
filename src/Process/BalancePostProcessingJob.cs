using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SC2Balance.Ingest;
using SC2Balance.Models;
using Newtonsoft.Json;

namespace Sc2Balance.Process
{
    public class BalancePostProcessingJob : AbstractPostProcessingJob
    {
        public override string JobType
        {
            get { return PostProcessingJobType.BALANCE.ToString();  }
        }

        public override void Run()
        {
            var uniqueGamesInTimeSpan = GetUniqueGamesInTimeSpan(Week);

            var terranWinRate = GetWinRateFromUniqueGames(uniqueGamesInTimeSpan, "TERRAN");
            var protossWinRate = GetWinRateFromUniqueGames(uniqueGamesInTimeSpan, "PROTOSS");
            var zergWinRate = GetWinRateFromUniqueGames(uniqueGamesInTimeSpan, "ZERG");

            var winRate = new WinRate
            {
                TerranWinRate = terranWinRate,
                ProtossWinRate = protossWinRate,
                ZergWinRate = zergWinRate
            };

            var json = JsonConvert.SerializeObject(winRate);
            Save(json);
        }
    }
}
