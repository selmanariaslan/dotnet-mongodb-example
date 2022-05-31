using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arch.CoreLibrary.Entities.Log
{
    public class ExceptionLog : EntityBase
    {
        public string Environment { get; set; }
        public string HostName { get; set; }
        public string LogLevel { get; set; }
        public string MethodName { get; set; }
        public string ExceptionMessage { get; set; }
        public string StackTrace { get; set; }
        public string Details { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public string Url { get; set; }
        public string Username { get; set; }
    }
}
