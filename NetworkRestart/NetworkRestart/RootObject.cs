using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkRestart
{
    public class RootObject
    {
        public Action Action { get; set; }
        public ServerConfiguration ServerConfiguration { get; set; }
        public List<string> computersToIgnore { get; set; }
        public EmailConfiguration EmailConfiguration { get; set; }
    }
}
