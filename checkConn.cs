using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Mobideskv2
{
    class checkConn
    {
        static bool isNetworkAvailable;
        public static void StartCheck()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface nic in nics)
            {
                if ((nic.NetworkInterfaceType != NetworkInterfaceType.Loopback && nic.NetworkInterfaceType != NetworkInterfaceType.Tunnel) && nic.OperationalStatus == OperationalStatus.Up)
                {
                    isNetworkAvailable = true;
                }

            }
            Console.WriteLine("Network Availability: " + isNetworkAvailable);
            NetworkChange.NetworkAvailabilityChanged += new NetworkAvailabilityChangedEventHandler(NetworkChange_NetworkAvailabilityChanged);
        }

        private static void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            isNetworkAvailable = e.IsAvailable;

            if (isNetworkAvailable)
            {
                if (!monitorChanges.isServerMonitoringEnabled && Properties.Settings.Default.lastupdate!="")
                {
                    userServerFiles srvr = new userServerFiles();
                    srvr.updateSizeCount();
                    monitorChanges.start_srv();
                    
                }
                //total count & size
                //check for serverChanges
            }
            else
            {
                if(monitorChanges.isServerMonitoringEnabled){
                    monitorChanges.stop_srv();
                   
                }
            }
            Console.WriteLine("Network Availability: " + isNetworkAvailable);
            
        }

       
    }
}
