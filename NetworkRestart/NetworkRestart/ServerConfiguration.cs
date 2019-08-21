using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkRestart
{
    public class ServerConfiguration
    {
        public string domain { get; set; }
        public string domainAdminUserName { get; set; }
        public string domainAdminPassword { get; set; }
        public string rootOUdistinguishedName { get; set; }
        public string includeSubfolders { get; set; }
    }
}
