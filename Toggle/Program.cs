using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Toggle
{
    class Program
    {
        private static MachineAvailabilityServiceReference.IService1 AvailabilityService;

        static void Main(string[] args)
        {
            AvailabilityService = new MachineAvailabilityServiceReference.Service1Client();
            if (args.Length == 0) {
                Console.WriteLine("Usage: Toggle {init, login, logout, reset}");
            }
            else {
                Toggle(args[0]);
            }
            System.Threading.Thread.Sleep(1000);
        }

        public static async void Toggle(string option)
        {
            bool status;
            if (option == "init")
            {
                string mac = "";
                string ip = "";
                IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                if (nics == null || nics.Length < 1) {
                    Console.WriteLine("  No network interfaces found.");
                    return;
                }
                foreach (NetworkInterface adapter in nics) {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    PhysicalAddress address = adapter.GetPhysicalAddress();
                    byte[] bytes = address.GetAddressBytes();

                    if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                        adapter.Description.IndexOf("Virtual") < 0 &&
                        adapter.Description.IndexOf("VirtualBox") < 0) {
                        mac = address.ToString();
                        break;
                    }
                }
                IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                IPAddress[] addr = ipEntry.AddressList;
                foreach (IPAddress ipa in addr) {
                    if (ipa.ToString().IndexOf(".") > 0) {
                        ip = ipa.ToString();
                        break;
                    }
                }
                Console.Write(Environment.MachineName + "\nMAC: " + mac + "\nIP: " + ip + "\n");
                status = await AvailabilityService.InitializeAsync(Environment.MachineName, mac, ip);
                Console.WriteLine("INITIALIZE " + status);
            }
            else if (option == "login")
            {
                status = await AvailabilityService.LoginAsync(Environment.MachineName);
                Console.WriteLine("LOGIN " + status);
            }
            else if (option == "logout")
            {
                status = await AvailabilityService.LogoutAsync(Environment.MachineName);
                Console.WriteLine("LOGOUT " + status);
            }
            else if (option == "reset")
            {
                status = await AvailabilityService.ResetAsync(Environment.MachineName);
                Console.WriteLine("RESET " + status);
            }
            else
            {
                Console.WriteLine("INVALID OPTION. Possible arguments: {init, login, logout, reset}");
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
