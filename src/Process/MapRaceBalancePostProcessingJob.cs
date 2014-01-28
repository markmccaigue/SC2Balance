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
    public class MapRaceBalancePostProcessingJob : AbstractPostProcessingJob
    {
        public override string JobType
        {
            get { return "MAPRACEBALANCE"; }
        }

        public override void Run()
        {
            MapRaceWinRate[] mapRaceWinRate = null;

            using (var db = new DataContext())
            {
                var maps = db.UniqueGmMatches.Where(g => g.Type == "SOLO").Select(x => x.Map).Distinct().ToList();
                var uniqueGamesInTimeSpan = GetUniqueGamesInTimeSpan(Week);

                mapRaceWinRate = maps.Select(map => new MapRaceWinRate
                {
                    Map = map,
                    RaceWinRate = new RaceWinRate
                    {
                        TVPWinRate = GetWinRateFromUniqueGamesForRaces(uniqueGamesInTimeSpan, "TERRAN", "PROTOSS", map),
                        TVZWinRate = GetWinRateFromUniqueGamesForRaces(uniqueGamesInTimeSpan, "TERRAN", "ZERG", map),
                        ZVPWinRate = GetWinRateFromUniqueGamesForRaces(uniqueGamesInTimeSpan, "ZERG", "PROTOSS", map)
                    }
                }).Where(x => !x.RaceWinRate.IsEmpty()).ToArray();
            }

            var json = JsonConvert.SerializeObject(mapRaceWinRate);
            Save(json);
        }
    }
}
