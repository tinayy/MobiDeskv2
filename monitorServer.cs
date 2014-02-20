using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobideskv2
{
    class monitorServer
    {
        userServerFiles srvrFiles = new userServerFiles();
        sync syncThat = new sync();


        public void getServerChanges()
        {
            //threadTime check for updates
            if(checkUpdates()){
                if(Properties.Settings.Default.autoUpdate){
                    syncThat.update("2");
                }
                else
                {
                    //prompt user
                }
            }
           
        }
        private bool checkUpdates()
        {
            String[] files = srvrFiles.getserverFiles();

            if(files.Length>0){
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
