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
    public class RaceBalancePostProcessingJob : AbstractPostProcessingJob
    {
        public override string JobType
        {
            get { return PostProcessingJobType.RACEBALANCE.ToString(); }
        }

        public override void Run()
        {
            var uniqueGamesInTimeSpan = GetUniqueGamesInTimeSpan(Week);

            var tVPWinRate = GetWinRateFromUniqueGamesForRaces(uniqueGamesInTimeSpan, "TERRAN", "PROTOSS");
            var tVZWinRate = GetWinRateFromUniqueGamesForRaces(uniqueGamesInTimeSpan, "TERRAN", "ZERG");
            var zVPWinRate = GetWinRateFromUniqueGamesForRaces(uniqueGamesInTimeSpan, "ZERG", "PROTOSS");

            var raceWinRate = new RaceWinRate
            {
                TVPWinRate = tVPWinRate,
                TVZWinRate = tVZWinRate,
                ZVPWinRate = zVPWinRate
            };

            var json = JsonConvert.SerializeObject(raceWinRate);
            Save(json);
        }
    }
}
