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
        static string configFilePath = "C:\\Users\\carro\\Desktop\\ConfigFile-Wyndmoor.json";
        Program program = new Program();
        static void Main(string[] args)
        {
            Console.WriteLine("Enter config file path");
            Console.WriteLine(configFilePath);
            GetComputersToIgnore();
            GetJsonData();
            GetComputers();
        }

        public static RootObject GetJsonData()
        {
            using (StreamReader r = new StreamReader(configFilePath))
            {
                string json = r.ReadToEnd();
                var response = JsonConvert.DeserializeObject<RootObject>(json);
                return response;
            }
        }

        public static List<string> GetComputersToIgnore()
        {
            using (StreamReader r = new StreamReader(configFilePath))
            {
                string json = r.ReadToEnd();
                var response = JsonConvert.DeserializeObject<RootObject>(json);
                if (response == null) return new List<string>();
                var computersToIgnore = new List<string>(response.computersToIgnore);

                return computersToIgnore;
            }

        }

        public static List<string> GetComputers()
        {
            using (StreamReader r = new StreamReader(configFilePath))
            {
                string json = r.ReadToEnd();
                var response = JsonConvert.DeserializeObject<RootObject>(json);

                List<string> ComputerNames = new List<string>();

                DirectoryEntry entry = new DirectoryEntry("LDAP://Wyndmoorals.local");
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

                foreach (var i in ComputerNames)
                {
                    if (response.computersToIgnore.Contains(i))
                    {
                        ComputerNames.Remove(i);
                    }
                }

                return ComputerNames;
            }            
        }
    }
}