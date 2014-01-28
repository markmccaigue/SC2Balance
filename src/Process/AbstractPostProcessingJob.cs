using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SC2Balance.Models;
using SC2Balance.Ingest;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Net;
using System.Web;

namespace Sc2Balance.Process
{
    public abstract class AbstractPostProcessingJob : IPostProcessingJob
    {
        public abstract void Run();

        protected static readonly TimeSpan Week = new TimeSpan(7, 0, 0, 0);

        public virtual string JobType
        {
            get { throw new NotImplementedException(); }
        }

        protected void Save(string jsonResults)
        {
            using (var db = new DataContext())
            {
                var latestRun = db.ProcessingRuns.OrderByDescending(x => x.Id).First();
                var output = new PostProcessingOutput
                {
                    PostProcessingJobType = JobType,
                    ProcessingRun = latestRun,
                    ProcessingRunId = latestRun.Id,
                    JsonResults = jsonResults
                };
                db.PostProcessingOutputs.Add(output);
                db.SaveChanges();
            }
        }

        protected float GetWinRateFromUniqueGamesForRaces(IList<UniqueGmMatch> games, string race1, string race2, string map = null) //r1 vs r2
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

        protected float GetWinRateFromUniqueGames(IList<UniqueGmMatch> games, string race, string map = null)
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
        
        protected IList<UniqueGmMatch> GetUniqueGamesInTimeSpan(TimeSpan span)
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
                //Make sure that we only count matches once, no matter if they are in several ingestions
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

        protected BalanceHistoryPoint[] GetBalanceHistoryPointsFromUniqueGames(IList<UniqueGmMatch> games, string map = null)
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
    }
}
