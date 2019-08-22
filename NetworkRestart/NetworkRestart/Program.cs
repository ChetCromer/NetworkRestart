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

        public static void RestartComputers()
        {
            ManagementObjectCollection instances;
            List<string> ComputerNames = GetComputers();
            foreach (var i in ComputerNames)
            {
                try
                {
                    string computerPath = string.Format(@"\\{0}\root\cimv2", i);
                    ConnectionOptions options = new ConnectionOptions();
                    options.Impersonation = System.Management.ImpersonationLevel.Impersonate;
                    //options.EnablePrivilages = true;
                    //or
                    //options.Username = "username";
                    //options.Password = "password";
                    //options.Authority = "";
                    ManagementScope scope = new ManagementScope(computerPath, options);
                    scope.Connect();

                    ManagementPath osPath = new ManagementPath("Win32_OperatingSystem");
                    ManagementClass os = new ManagementClass(scope, osPath, null);
                    instances = os.GetInstances();


                }
                catch (Exception)
                {

                    throw;
                }

                foreach (ManagementObject instance in instances)
                {
                    object result = instance.InvokeMethod("Reboot", new object[] { });
                    uint returnValue = (uint)result;

                    if (returnValue != 0)
                    {
                        throw new Exception();
                    }
                }
            }          
        }
    }
}