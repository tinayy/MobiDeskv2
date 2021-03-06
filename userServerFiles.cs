﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobideskv2
{
    class userServerFiles
    {
        String reqData;
        String reqPage = "userFile.php";
        ftp server = new ftp();
        httprequest request = new httprequest();
       
        public decimal getTotalSize(String uid)
        {
            reqData = "action=getTotalSize&usrid="+uid;
            Console.WriteLine(request.Onrequest(reqPage, reqData));
            decimal res = decimal.Parse(request.Onrequest(reqPage, reqData));
            return res;
        }

        public int getTotalCount(String uid)
        {
            reqData = "action=getTotalCount&usrid="+uid;
            int res = int.Parse(request.Onrequest(reqPage,reqData));
            return res;
        }

        public void updateSizeCount()
        {
            Properties.Settings.Default.totalb = 0;
            Properties.Settings.Default.total = 0;
            Properties.Settings.Default.Save();
            initset settings = new initset();
            settings.updatesize(getTotalSize(Properties.Settings.Default.uid));

        }

        public void createServerFolder()
        {
            server.createFolder("");
        }

        public String[] getserverFiles()
        {
            String reqData = String.Format("action=getFiles&usrid={0}&lastupdate={1}",Properties.Settings.Default.uid,Properties.Settings.Default.lastupdate);
            String[] serverfiles = request.Onrequest("userFile.php",reqData).Trim().Split('\n');
            return serverfiles;
        }

        private  String[] _getserverChanges()
        {
            
            String reqData = String.Format("action=getChanges&usrid={0}&lastupdate={1}", Properties.Settings.Default.uid, Properties.Settings.Default.lastupdate);
             String[] serverfileChanges = request.Onrequest("userFile.php", reqData).Trim().Split('\n');
            
             return serverfileChanges;
           
        }

        public String[] getserverChanges
        {
            get
            { return _getserverChanges(); }
            set
            {
                value = _getserverChanges();
            }
        }

        
    }
}
