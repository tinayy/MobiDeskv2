using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mobideskv2
{
    class initset
    {
        userServerFiles server = new userServerFiles();
        userdevice dvc = new userdevice();
        ftp ftp = new ftp();
        public bool settings(String option,String data)
        {
            switch(option){
                case"createSet":
                    dvc.createSettings();
                break;
                case"clearSet":
                    clearsettings();
                break;
                case"setInitProperties":
                    setinitProperties(data);
                break;
            }

            return true;
        }

        private void setinitProperties(String toJson)
        {
            var settings = Properties.Settings.Default; 
            JToken token = JObject.Parse(toJson);
            String uid = (String)token.SelectToken("usrid");
            decimal size = server.getTotalSize(uid);
            int count = server.getTotalCount(uid);

            switch ((String)token.SelectToken("package"))
            {
                case "STND":
                    settings.usertype = "Standard User";
                    settings.maxsize = 5;
                    break;
                case "PREM":
                    settings.usertype = "Premium User";
                    settings.maxsize = 10;
                    break;
                case "ULTM":
                    settings.usertype = "Ultimate User";
                    settings.maxsize = 15;
                    break;
            }

            System.Windows.Forms.MessageBox.Show("" + Properties.Settings.Default.totalb);
            updatesize(size);

            settings.uid = uid;
            settings.filecount = count;
            settings.uemail = (String)token.SelectToken("eadd");
            settings.ufname = (String)token.SelectToken("fname");
            Properties.Settings.Default.Save();

            Console.WriteLine("Logged in. User id: " + settings.uid);
            //ftp.createFolder("");

            System.Windows.Forms.MessageBox.Show("Size: " + settings.total+" "+settings.fsizeunit);
        }

        public void updatesize(decimal size)
        {
            var settings = Properties.Settings.Default; 
            decimal nsize = size + Properties.Settings.Default.totalb;
          
            if (nsize < 1024)
            {
                settings.total = nsize;
                settings.fsizeunit = "KB";
                //System.Windows.Forms.MessageBox.Show("B");
                //0kb
            }
            else if (nsize < (1024 * 1024) && nsize >= 1024)
            {
                //System.Windows.Forms.MessageBox.Show("KB");
                settings.total = Math.Round(nsize / 1024, 2);  //kb
                settings.fsizeunit = "KB";
            }
            else if (nsize >= (1024 * 1024) && nsize < (1024 * 1024 * 1024))
            {
                //System.Windows.Forms.MessageBox.Show("MB");
                settings.total = Math.Round(nsize / 1024 / 1024, 2);
                settings.fsizeunit = "MB"; //mb
            }
            else if (nsize >= (1024 * 1024 * 1024))
            {
                //System.Windows.Forms.MessageBox.Show("GB");
                settings.total = Math.Round(nsize / 1024 / 1024 / 1024, 2);
                settings.fsizeunit = "GB"; //GB
            }
                settings.totalb = nsize;
                settings.Save();
                updatesizebar();
        }

        private void updatesizebar()
        {
            decimal totalbytes = Properties.Settings.Default.totalb;
            decimal totalmaxbytes = (1073741824UL * (ulong)Properties.Settings.Default.maxsize);
            decimal perc = Math.Round((totalbytes / totalmaxbytes) * 100,2);
            Properties.Settings.Default.perc = perc;
            Properties.Settings.Default.Save();
            Console.WriteLine("percent: "+perc);
        }

        private void clearsettings()
        {
            try
            {
                Properties.Settings.Default.Reset();
            }
            catch(Exception e){
                throw e;
            }
        }

        
    }
}
