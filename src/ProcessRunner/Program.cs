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
            new ProcessingManager().RunUniqueGamesPass();
        }
    }
}
