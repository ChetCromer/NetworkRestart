using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkRestart
{
    public class EmailConfiguration
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string fromAddress { get; set; }
        public List<string> toAddresses { get; set; }
        public bool sendErrorsOnly { get; set; }
    }
}
