using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkRestart
{
    public class JsonResponse
    {
        public Action Action { get; set; }
        public EmailConfiguration EmailConfiguration { get; set; }
        public ServerConfiguration ServerConfiguration { get; set; }
        public RootObject RootObject { get; set; }
    }
}
