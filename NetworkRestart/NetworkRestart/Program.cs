using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkRestart
{
    public class Program
    {
        static string configFilePath = "C:\\NetworkRestart\\samples\\sampleconfigfile.json";
        Program program = new Program();
        static void Main(string[] args)
        {
            var configFilePath = "C:\\NetworkRestart\\samples\\sampleconfigfile.json";
            Console.WriteLine("Enter config file path");
            Console.WriteLine(configFilePath);
            LoadJson();
            Console.ReadKey();
        }

        public static void LoadJson()
        {
            using (StreamReader r = new StreamReader(configFilePath))
            {
                string json = r.ReadToEnd();
                var response = JsonConvert.DeserializeObject<RootObject>(json);
                Console.WriteLine(response);
            }
        }
    }
}
