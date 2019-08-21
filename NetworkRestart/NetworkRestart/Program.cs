using Newtonsoft.Json;
using System;
using System.Management;
using System.IO;
using System.DirectoryServices;
using System.Collections.Generic;

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

        public static List<string> GetComputers()
        {
            List<string> ComputerNames = new List<string>();

            DirectoryEntry entry = new DirectoryEntry("OU=xxxx,OU=Computers,OU=Production,DC=xxxx,DC=local");
            DirectorySearcher mySearcher = new DirectorySearcher(entry);
            mySearcher.Filter = ("(objectClass=computer)");
            mySearcher.SizeLimit = int.MaxValue;
            mySearcher.PageSize = int.MaxValue;

            foreach (SearchResult resEnt in mySearcher.FindAll())
            {
                //"CN=SGSVG007DC"
                string ComputerName = resEnt.GetDirectoryEntry().Name;
                if (ComputerName.StartsWith("CN="))
                    ComputerName = ComputerName.Remove(0, "CN=".Length);
                ComputerNames.Add(ComputerName);
            }

            mySearcher.Dispose();
            entry.Dispose();

            return ComputerNames;
        }
    }
}
