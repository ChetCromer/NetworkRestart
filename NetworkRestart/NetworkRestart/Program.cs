using Newtonsoft.Json;
using System;
using System.Management;
using System.IO;
using System.DirectoryServices;
using System.Collections.Generic;
using System.Diagnostics;

namespace NetworkRestart
{
    public class Program
    {
        static string configFilePath = "C:\\Users\\Carr O'Connor\\Desktop\\ConfigFile-Wyndmoor.json";
        Program program = new Program();

        static void Main(string[] args)
        {
            // Create a writer and open the file:           
            Console.WriteLine("Enter config file path");
            Console.WriteLine(configFilePath);
            RestartComputers();
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

        public static List<string> GetComputers()
        {
            var response = GetJsonData();
            List<string> ComputerNames = new List<string>();

            DirectoryEntry entry = new DirectoryEntry("LDAP://Wyndmoorals.local/" + response.ServerConfiguration.rootOUdistinguishedName);
            entry.Username = "c2itadmin@wyndmooralf.com";
            entry.Password = "jJRtMRAHOI@167h";
            DirectorySearcher mySearcher = new DirectorySearcher(entry);
            mySearcher.Filter = ("(objectClass=computer)");
            mySearcher.SizeLimit = int.MaxValue;
            mySearcher.PageSize = int.MaxValue;

            foreach (SearchResult resEnt in mySearcher.FindAll())
            {
                string ComputerName = resEnt.GetDirectoryEntry().Name;
                if (ComputerName.StartsWith("CN="))
                    ComputerName = ComputerName.Remove(0, "CN=".Length);
                ComputerNames.Add(ComputerName);
            }

            mySearcher.Dispose();
            entry.Dispose();

            foreach (var i in ComputerNames.ToArray())
            {
                if (response.computersToIgnore.Contains(i))
                {
                    ComputerNames.Remove(i);
                }
            }

            return ComputerNames;
        }

        public static void RestartComputers()
        {
            ManagementObjectCollection instances;
            List<string> ComputerNames = GetComputers();
            StreamWriter log;
            var response = GetJsonData();

            log = File.AppendText("C:\\Users\\Carr O'Connor\\Desktop\\logfile.txt");
            log.WriteLine("TIMESTAMP :" + DateTime.Now);
            log.WriteLine("ACTION TAKEN :" + response.Action.action);

            foreach (var i in ComputerNames)
            {
                try
                {
                    ConnectionOptions options = new ConnectionOptions();
                    options.Authority = "ntlmdomain:WYNDMOORALS.LOCAL";
                    options.Username = "c2itadmin";
                    options.Password = "jJRtMRAHOI@167h";
                    options.EnablePrivileges = true;

                    ManagementScope scope = new ManagementScope("\\\\" + i + "\\root\\CIMV2", options);
                    scope.Connect();

                    ManagementPath osPath = new ManagementPath("Win32_OperatingSystem");
                    ManagementClass os = new ManagementClass(scope, osPath, null);
                    instances = os.GetInstances();
                }

                catch (Exception e)
                {
                    log.WriteLine(i + e);
                    continue;
                }

                foreach (ManagementObject instance in instances)
                {
                    Console.WriteLine("hit computer " + instance);
                    log.WriteLine("Action taken on " + instance);

                    object result = instance.InvokeMethod("Reboot", new object[] { });
                    uint returnValue = (uint)result;

                    if (returnValue != 0)
                    {
                        log.WriteLine("Action NOT taken on " + instance);
                        Console.WriteLine("Did not restart");
                        log.WriteLine();
                    }
                }

                // Close the stream:
            }
            log.Close();
        }
    }
}