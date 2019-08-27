using Newtonsoft.Json;
using System;
using System.Management;
using System.IO;
using System.DirectoryServices;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Net.Mail;

namespace NetworkRestart
{
    public class Program
    {
        static string configFilePath;
        Program program = new Program();
        static string fileName;
        static string OUpath;
        static bool hasError = false;

        static void Main(string[] args)
        {
            if (args[0] == null)
            {
                Console.WriteLine("Please provide a valid config file.");
            }
            else
            {
                configFilePath = args[0];
                ActionComputers();
            }        
        }

        public static RootObject GetJsonData()
        {
            //Grab data from config file
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
            OUpath = response.ServerConfiguration.rootOUdistinguishedName;
            //Connect to active directory and filter results
            DirectoryEntry entry = new DirectoryEntry(OUpath);
            entry.Username = response.ServerConfiguration.domainAdminUserName;
            entry.Password = response.ServerConfiguration.domainAdminPassword;
            DirectorySearcher mySearcher = new DirectorySearcher(entry);
            if (response.ServerConfiguration.includeSubfolders == false)
            {
                mySearcher.SearchScope = System.DirectoryServices.SearchScope.OneLevel;
            }
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

            //remove computers in ignore list
            foreach (var i in ComputerNames.ToArray())
            {
                if (response.computersToIgnore.Contains(i))
                {
                    ComputerNames.Remove(i);
                }
            }

            return ComputerNames;
        }

        public static void ActionComputers()
        {
            ManagementObjectCollection instances;
            List<string> ComputerNames = GetComputers();
            var response = GetJsonData();

            //Create log file
            fileName = $"LogFile_{response.Action.action}.txt";
            using (StreamWriter log = new StreamWriter(fileName))
            {
                log.WriteLine("TIMESTAMP :" + DateTime.Now);
                log.WriteLine("ACTION TAKEN :" + response.Action.action);

                foreach (var i in ComputerNames)
                {
                    try
                    {
                        //Connecting to individual machines with config credentials
                        ConnectionOptions options = new ConnectionOptions();
                        options.Authority = "ntlmdomain:" + response.ServerConfiguration.domain;
                        options.Username = response.ServerConfiguration.domainAdminUserName;
                        options.Password = response.ServerConfiguration.domainAdminPassword;
                        options.EnablePrivileges = true;

                        ManagementScope scope = new ManagementScope("\\\\" + i + "\\root\\CIMV2", options);
                        scope.Connect();

                        ManagementPath osPath = new ManagementPath("Win32_OperatingSystem");
                        ManagementClass os = new ManagementClass(scope, osPath, null);
                        instances = os.GetInstances();
                    }

                    catch (Exception e)
                    {
                        hasError = true;
                        Console.WriteLine($"Exception on {i}: {e.Message}");
                        log.WriteLine($"Exception on {i}: {e.Message}");
                        continue;
                    }
                     

                    foreach (ManagementObject instance in instances)
                    {
                        ManagementBaseObject inParams = instance.GetMethodParameters("Win32Shutdown");
                        inParams["Flags"] = 0;
                        if (response.Action.action.ToLower() == "logoff")
                        {
                            try
                            {
                                //logoff action using WMI
                                object result = instance.InvokeMethod("Win32Shutdown", inParams, null);
                                Console.WriteLine("Successful logoff attempt on " + i);
                                log.WriteLine("Successful logoff attempt on " + i);
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("Unsuccessful logoff attempt on " + i);
                                log.WriteLine("Unsuccessful logoff attempt on " + i);
                                continue;
                            }

                        }
                        else
                        {
                            //Every other action using WMI
                            object result = instance.InvokeMethod(response.Action.action , new object[] { });

                            uint returnValue = (uint)result;

                            if (returnValue != 0)
                            {
                                hasError = true;
                                Console.WriteLine("Action NOT taken on " + i);
                                log.WriteLine("Action NOT taken on " + i);
                            }
                            else
                            {
                                Console.WriteLine("Action taken on " + i);
                                log.WriteLine("Action taken on " + i);
                            }
                        }                       
                    }
                }
            }
            if (hasError == true)
            {
                try
                {
                    //Send email if errors were found
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient(response.EmailConfiguration.Server);
                    SmtpServer.UseDefaultCredentials = true;
                    string str = string.Empty;
                    foreach (var item in response.EmailConfiguration.toAddresses)
                    {
                        mail.To.Add(item);
                    }

                    mail.From = new MailAddress(response.EmailConfiguration.fromAddress);
                    mail.Subject = "LogFile" + DateTime.Now;
                    mail.Body = "attached log file";

                    System.Net.Mail.Attachment attachment;
                    attachment = new System.Net.Mail.Attachment(fileName);
                    mail.Attachments.Add(attachment);

                    SmtpServer.Port = response.EmailConfiguration.Port;
                    SmtpServer.Credentials = new System.Net.NetworkCredential(response.EmailConfiguration.userName, response.EmailConfiguration.password);
                    SmtpServer.EnableSsl = true;

                    SmtpServer.Send(mail);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}