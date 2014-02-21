using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Mobideskv2
{
    class monitorServer
    {
        static Timer timer;
        //private static bool _isTimerEnabled;
        public static bool isTimerEnabled
        {
            get;
            set;
        }


        public  static void start()
        {
            timer = new Timer(2000);
            timer.Elapsed += new ElapsedEventHandler(monitorServer.detectChanges);
            timer.Start();
            isTimerEnabled = true;
            Console.WriteLine("Timer started");
        }

        public static void stop()
        {
            timer.Stop();
            isTimerEnabled = false;
            Console.WriteLine("Timer stopped");
        }

        static void detectChanges(object o, ElapsedEventArgs e)
        {
            userServerFiles srvrFiles = new userServerFiles();
            String[] changes = srvrFiles.getserverChanges;
            if(changes.Length>0){
                stop();
                Console.WriteLine("There are changes");
                QueueChanges(changes);
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
                }
            }
            finally
            {
                //process queue
            }
        }
      
    }
}
