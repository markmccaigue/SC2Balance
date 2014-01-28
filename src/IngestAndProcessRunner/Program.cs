using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SC2Balance.Ingest;
using Sc2Balance.Process;

namespace SC2Balance.IngestAndProcessRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                new IngestionManager().RunNewIngestion();

                var jobs = new List<IPostProcessingJob>
                {
                    new BalancePostProcessingJob(),
                    new BalanceHistoryPostProcessingJob(),
                    new MapBalancePostProcessingJob(),
                    //new MapBalanceHistoryPostProcessingJob(),
                    new MapRaceBalancePostProcessingJob(),
                    new RaceBalancePostProcessingJob()
                };

                var processingManager = new ProcessingManager();
                processingManager.RunUniqueGamesPass();
                processingManager.RunPostProcessingJobs(jobs);
            }
            catch (Exception exception)
            {
                Logger.LogError(exception);
                System.Environment.Exit(exception.HResult);
            }
        }
    }
}
