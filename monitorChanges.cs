using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Mobideskv2
{
    class monitorChanges
    {
        static Timer timer_srv;
        static Timer timer_loc;
        //private static bool _isTimerEnabled;
        public static bool isServerMonitoringEnabled
        {
            get;
            set;
        }
        public static bool isLocalMonitoringEnabled
        {
            get;
            set;
        }


        public  static void start_srv()
        {
            timer_srv = new Timer(4000);
            timer_srv.Elapsed += new ElapsedEventHandler(monitorChanges.detectChanges_srv);
            timer_srv.Start();
            isServerMonitoringEnabled = true;
            Console.WriteLine("Timer started");
        }

        public static void stop_srv()
        {
            isServerMonitoringEnabled = false;
            timer_srv.Stop();
            Console.WriteLine("Timer stopped");
        }

        static void detectChanges_srv(object o, ElapsedEventArgs e)
        {
            userServerFiles srvrFiles = new userServerFiles();
            String[] changes = srvrFiles.getserverChanges;
            if(changes.Length>0 && !changes.Contains("")){
                stop_srv();
                stop_loc();
                Console.WriteLine("There are changes");
                QueueChanges(changes);
                objects.processQueue("stl");
            }
            else
            {
                Console.WriteLine("No Changes");
            }
        }

        static void QueueChanges(String[] changes)
        {
            try
            {
                foreach(String s in changes){
                    objects.queueFrmServer.Enqueue(s.Trim());
                    Console.WriteLine(s);
                }
            }
            finally
            {
                
            }
        }


        public static void start_loc()
        {
            timer_loc = new Timer(4000);
            timer_loc.Elapsed += new ElapsedEventHandler(monitorChanges.detectChanges_loc);
            timer_loc.Start();
            isLocalMonitoringEnabled = true;
            Console.WriteLine("Local monitoring started");
        }

        public static void stop_loc()
        {
            isLocalMonitoringEnabled = false;
            timer_loc.Stop();
            Console.WriteLine("Local monitoring stopped");
        }

        static void detectChanges_loc(object o, ElapsedEventArgs e)
        {
            if (objects.queue.Count > 0)
            {
                monitorChanges.stop_loc();
                monitorChanges.stop_srv();
                Console.WriteLine("Changes on local");
                objects.processQueue("lts");
                //MainWindow mw = new MainWindow();
                //mw.updatePanelVisible(1);
            }
            else
            {
                Console.WriteLine("No changes on local");
            }
        }
      
    }
}
