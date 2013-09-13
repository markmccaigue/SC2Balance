using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SC2Balance.Ingest;
using SC2Balance.Models;

namespace Sc2Balance.IngestRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            new IngestionManager().RunNewIngestion();
        }
    }
}
