using Newtonsoft.Json;
using System;
using System.Management;
using System.IO;

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

            ConnectionOptions options = new ConnectionOptions();
            options.Impersonation = System.Management.ImpersonationLevel.Impersonate;

            ManagementScope scope = new ManagementScope("\\\\xxxx\\root\\cimv2", options);
            scope.Connect();

            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

            ManagementObjectCollection queryCollection = searcher.Get();
            foreach (ManagementObject m in queryCollection)
            {
                // Display the remote computer information
                Console.WriteLine("Computer Name     : {0}", m["csname"]);
                Console.WriteLine("Windows Directory : {0}", m["WindowsDirectory"]);
                Console.WriteLine("Operating System  : {0}", m["Caption"]);
                Console.WriteLine("Version           : {0}", m["Version"]);
                Console.WriteLine("Manufacturer      : {0}", m["Manufacturer"]);
            }
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
