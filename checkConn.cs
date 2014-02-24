using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Timers;


namespace Mobideskv2
{
    class checkConn
    {
        static bool isNetworkAvailable;
        static Timer connWatch;
        public static bool isConnectionAvailable
        {
            get;
            set;
        }

        public static void startConnWatch()
        {
            connWatch = new Timer(4000);
            connWatch.Elapsed += new ElapsedEventHandler(checkConn.CheckForInternetConnection);
            connWatch.Start();


        }

        private static void CheckForInternetConnection(object o, ElapsedEventArgs e)
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = client.OpenRead("http://www.google.com"))
                {
                    Console.WriteLine("Connection available");
                    checkConn.isConnectionAvailable = true;
                }
            }
            catch
            {
                Console.WriteLine("Connection unavailable");
                checkConn.isConnectionAvailable = false;
            }
        }

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
            Console.WriteLine("Network Availability: " + isNetworkAvailable);

            if (isNetworkAvailable)
            {
                if (monitorChanges.isServerMonitoringEnabled && Properties.Settings.Default.lastupdate!="")
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
            
            
        }

       
    }
}
