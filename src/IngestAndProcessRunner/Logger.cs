using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SC2Balance.IngestAndProcessRunner
{
    public static class Logger
    {
        private static void Log(string message)
        {
            var path = string.Format("{0}{1}{2}{3}", System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), Path.DirectorySeparatorChar, DateTime.UtcNow.ToString("ddMMyyHHmmssffffff"), ".txt");
            File.WriteAllText(path, message);
        }

        public static void LogError(Exception exception)
        {
            if (exception == null)
            {
                return;
            }
            var message = String.Format("{0}{1}{2}{3}{4}{5}", exception.Message, Environment.NewLine, exception.Source, Environment.NewLine, exception.StackTrace, Environment.NewLine);
            Log(message);
            LogError(exception.InnerException);
        }
    }
}
