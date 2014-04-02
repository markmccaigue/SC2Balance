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
    public class MapBalancePostProcessingJob : AbstractPostProcessingJob
    {
        public override string JobType
        {
            get { return PostProcessingJobType.MAPBALANCE.ToString(); }
        }

        public override void Run()
        {
            MapWinRate[] mapWinRate = null;

            using (var db = new DataContext())
            {
                var maps = db.UniqueGmMatches.Where(g => g.Type == "SOLO").Select(x => x.Map).Distinct().ToList();
                var uniqueGamesInTimeSpan = GetUniqueGamesInTimeSpan(Week);

                mapWinRate = maps.Select(map => new MapWinRate
                {
                    Map = map,
                    WinRate = new WinRate
                    {
                        TerranWinRate = GetWinRateFromUniqueGames(uniqueGamesInTimeSpan, "TERRAN", map),
                        ProtossWinRate = GetWinRateFromUniqueGames(uniqueGamesInTimeSpan, "PROTOSS", map),
                        ZergWinRate = GetWinRateFromUniqueGames(uniqueGamesInTimeSpan, "ZERG", map)
                    }
                }).Where(x => !x.WinRate.IsEmpty()).ToArray();
            }

            var json = JsonConvert.SerializeObject(mapWinRate);
            Save(json);
        }
    }
}
