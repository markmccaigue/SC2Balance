using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace System
{
    [Serializable]
    public class JavaScriptException : Exception
    {
        public JavaScriptException(string message, string errorUrl, int lineNumber)
            : base(message)
        {
            this.Source = string.Format(@"at {0}:line {1}", errorUrl, lineNumber);
        }

        public override string ToString()
        {
            return this.Message + System.Environment.NewLine + "    " + this.Source;
        }
    }
}