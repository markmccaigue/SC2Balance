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
                new ProcessingManager().RunUniqueGamesPass();
            }
            catch (Exception exception)
            {
                Logger.LogError(exception);
                System.Environment.Exit(exception.HResult);
            }
        }
    }
}
