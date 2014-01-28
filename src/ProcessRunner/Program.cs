using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sc2Balance.Process;

namespace ProcessRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            var jobs = new List<IPostProcessingJob>
            {
                new BalancePostProcessingJob(),
                new BalanceHistoryPostProcessingJob(),
                new MapBalancePostProcessingJob(),
                new MapBalanceHistoryPostProcessingJob(),
                new MapRaceBalancePostProcessingJob(),
                new RaceBalancePostProcessingJob()
            };

            var start = DateTime.UtcNow;
            var processingManager = new ProcessingManager();
            processingManager.RunUniqueGamesPass();
            processingManager.RunPostProcessingJobs(jobs);
            var time = DateTime.UtcNow - start;

            Console.WriteLine(time.TotalSeconds);
            Console.ReadLine();
        }
    }
}
