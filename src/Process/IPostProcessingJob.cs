using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using SC2Balance.Ingest;

namespace Sc2Balance.Process
{
    public interface IPostProcessingJob
    {
        void Run(DataContext db);
    }
}
