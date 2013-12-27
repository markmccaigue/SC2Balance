using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using SC2Balance.Models;
using SC2Balance.Ingest;
using SC2Balance.ViewModels;

namespace SC2Balance.Controllers
{
    public class DataController : ApiController
    {
        # region private

        private static readonly TimeSpan Week = new TimeSpan(7, 0, 0, 0);

        private float GetWinRateFromUniqueGamesForRaces(IList<UniqueGmMatch> games, string race1, string race2, string map = null) //r1 vs r2
        {
            var mapGamesPlayed = map == null ? games : games.Where(g => g.Map == map);
            var raceGamesPlayed =
                mapGamesPlayed.Where(
                    u => (u.Type == GameType.SOLO.ToString()) && (u.LadderMember1.FavoriteRaceP1 == race1 && u.LadderMember2.FavoriteRaceP1 == race2) ||
                         (u.LadderMember1.FavoriteRaceP1 == race2 && u.LadderMember2.FavoriteRaceP1 == race1)).ToList();

            float raceGamesCount = raceGamesPlayed.Count();
            float raceWinCount = raceGamesPlayed.Count(g => g.Winner.FavoriteRaceP1 == race1);
            if (raceWinCount < 1)
            {
                return 0;
            }
            var raceWinPercentage = (raceWinCount / raceGamesCount) * 100;
            return raceWinPercentage;
        }
        
        private float GetWinRateFromUniqueGames(IList<UniqueGmMatch> games, string race, string map = null)
        {
            var mapGamesPlayed = map == null ? games : games.Where(g => g.Map == map);
            var raceGamesPlayed =
                mapGamesPlayed.Where(
                    u => (u.Type == GameType.SOLO.ToString()) && (u.LadderMember1.FavoriteRaceP1 == race || u.LadderMember2.FavoriteRaceP1 == race) &&
                         !(u.LadderMember1.FavoriteRaceP1 == race && u.LadderMember2.FavoriteRaceP1 == race)).ToList();

            float raceGamesCount = raceGamesPlayed.Count();
            float raceWinCount = raceGamesPlayed.Count(g => g.Winner.FavoriteRaceP1 == race);
            if (raceWinCount < 1)
            {
                return 0;
            }
            var raceWinPercentage = (raceWinCount / raceGamesCount) * 100;
            return raceWinPercentage;
        }
        //TODO: Indexes for performance? Test
        private IList<UniqueGmMatch> GetUniqueGamesInTimeSpan(TimeSpan span)
        {
            using (var db = new DataContext())
            {
                var currentTime = DateTime.UtcNow;
                var earliestTime = (currentTime.Ticks - span.Ticks < DateTime.MinValue.Ticks) ? DateTime.MinValue : (currentTime - span);

                var ingestionIds = db.Ingestions.Where(i => i.Time > earliestTime).Select(i => i.Id);
                var gamesInTimeSpan =
                    db.UniqueGmMatches.Include(x => x.LadderMember1)
                        .Include(x => x.LadderMember2)
                        .Include(x => x.Winner)
                        .Where(m => ingestionIds.Contains(m.IngestionId))
                        .ToList();
                var uniqueGamesInTimeSpan = new List<UniqueGmMatch>();
                foreach (var match in gamesInTimeSpan)
                {
                    if (uniqueGamesInTimeSpan.Any(u => DateTime.Equals(u.DateTime, match.DateTime)))
                    {
                        continue;
                    }

                    uniqueGamesInTimeSpan.Add(match);
                }

                return uniqueGamesInTimeSpan;
            }
        }

        private BalanceHistoryPoint[] GetBalanceHistoryPointsFromUniqueGames(IList<UniqueGmMatch> games, string map = null)
        {
            using (var db = new DataContext())
            {
                var ingestions = db.Ingestions.ToList();

                var points = new List<BalanceHistoryPoint>();

                foreach (var ingestion in ingestions)
                {
                    var ingestionId = ingestion.Id;
                    var ingestionGames = games.Where(g => g.IngestionId == ingestionId).ToList();

                    points.Add(new BalanceHistoryPoint
                    {
                        DateTime = ingestion.Time,
                        WinRate = new WinRate
                        {
                            TerranWinRate = GetWinRateFromUniqueGames(ingestionGames, "TERRAN", map),
                            ProtossWinRate = GetWinRateFromUniqueGames(ingestionGames, "PROTOSS", map),
                            ZergWinRate = GetWinRateFromUniqueGames(ingestionGames, "ZERG", map),
                        }
                    });
                }

                return points.ToArray();
            }
        }
        #endregion

        //Rename the API that mentions a time span
        //Long term plan is to take in a param for this

        // GET api/Default1
        public WinRate GetBalanceForSevenDays()
        {
            var uniqueGamesInTimeSpan = GetUniqueGamesInTimeSpan(Week);

            var terranWinRate = GetWinRateFromUniqueGames(uniqueGamesInTimeSpan, "TERRAN");
            var protossWinRate = GetWinRateFromUniqueGames(uniqueGamesInTimeSpan, "PROTOSS");
            var zergWinRate = GetWinRateFromUniqueGames(uniqueGamesInTimeSpan, "ZERG");

            return new WinRate
            {
                TerranWinRate = terranWinRate,
                ProtossWinRate = protossWinRate,
                ZergWinRate = zergWinRate
            };
        }

        public RaceWinRate GetRaceBalanceForSevenDays()
        {
            var uniqueGamesInTimeSpan = GetUniqueGamesInTimeSpan(Week);

            var tVPWinRate = GetWinRateFromUniqueGamesForRaces(uniqueGamesInTimeSpan, "TERRAN", "PROTOSS");
            var tVZWinRate = GetWinRateFromUniqueGamesForRaces(uniqueGamesInTimeSpan, "TERRAN", "ZERG");
            var zVPWinRate = GetWinRateFromUniqueGamesForRaces(uniqueGamesInTimeSpan, "ZERG", "PROTOSS");

            return new RaceWinRate
            {
                TVPWinRate = tVPWinRate,
                TVZWinRate = tVZWinRate,
                ZVPWinRate = zVPWinRate
            };
        }

        public MapBalanceHistoryPoints[] GetMapBalanceHistory()
        {
            var uniqueGames = GetUniqueGamesInTimeSpan(TimeSpan.MaxValue);

            using (var db = new DataContext())
            {
                var maps = db.UniqueGmMatches.Where(g => g.Type == GameType.SOLO.ToString()).Select(x => x.Map).Distinct().ToList();

                return maps.Select(map => new MapBalanceHistoryPoints
                {
                    Map = map,
                    BalanceHistoryPoints = GetBalanceHistoryPointsFromUniqueGames(uniqueGames, map)
                }).ToArray();
            }
        }

        public BalanceHistoryPoint[] GetBalanceHistory()
        {
            var uniqueGames = GetUniqueGamesInTimeSpan(TimeSpan.MaxValue);
            var points = GetBalanceHistoryPointsFromUniqueGames(uniqueGames);
            return points;
        }

        public int GetHoursSinceUpdate()
        {
            using (var db = new DataContext())
            {
                var lastUpdated = db.ProcessingRuns.OrderByDescending(r => r.Id).First().DateTime;
                return (int)TimeSpan.FromTicks(DateTime.UtcNow.Ticks - lastUpdated.Ticks).TotalHours;
            }
        }

        public MapRaceWinRate[] GetMapRaceBalanceForSevenDays()
        {
            using (var db = new DataContext())
            {
                var maps = db.UniqueGmMatches.Where(g => g.Type == GameType.SOLO.ToString()).Select(x => x.Map).Distinct().ToList();
                var uniqueGamesInTimeSpan = GetUniqueGamesInTimeSpan(Week);

                return maps.Select(map => new MapRaceWinRate
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
        }

        public MapWinRate[] GetMapBalanceForSevenDays()
        {
            using (var db = new DataContext())
            {
                var maps = db.UniqueGmMatches.Where(g => g.Type == GameType.SOLO.ToString()).Select(x => x.Map).Distinct().ToList();
                var uniqueGamesInTimeSpan = GetUniqueGamesInTimeSpan(Week);

                return maps.Select(map => new MapWinRate
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
        }
    }
}
//TODO: Convert magic strings to enum