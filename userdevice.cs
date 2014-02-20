using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Management;
using System.Windows.Forms;

namespace Mobideskv2
{
    class userdevice
    {
        httprequest request = new httprequest();
        String reqPage = "userDvc.php";
        String reqData;
       

        public String getdefaultloc()
        {         
            String loc = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            String path = Path.Combine(loc,"");
            return path;
        }

        public String getcompname()
        {
            String comname = Environment.MachineName;
            return comname;
        }

        public void setInitialSettings()
        {
           
            Properties.Settings.Default.directorypath = getdefaultloc();
            Properties.Settings.Default.computername = getcompname();
            Properties.Settings.Default.Save();        
        }

        public bool createSettings()
        {     
                if(deviceExists()){
                MessageBox.Show("Update settings");
                return updateDvcRecord();
            }
            else
            {
                MessageBox.Show("Create settings");
                bool res = createDvcRecord();
                Properties.Settings.Default.deviceid = getDeviceId();
                return res;
            }
            
        }

        private String getDeviceId()
        {
            reqData = String.Format("action=getDvcId&usrid={0}&macprodid={1}",Properties.Settings.Default.uid,macProdId());
            String dvcid = request.Onrequest(reqPage,reqData);
            Properties.Settings.Default.deviceid = dvcid;
            return dvcid;
        }

        private bool deviceExists()
        {
            try { 
                reqData = String.Format("action=checkDvcExists&usrid={0}&macprodid={1}",Properties.Settings.Default.uid,macProdId());
                if (!request.Onrequest(reqPage, reqData).Equals(""))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception e){
                MessageBox.Show(e.ToString());
                return false;
            }
        }

        private bool createDvcRecord()
        {

            String usrid = Properties.Settings.Default.uid, compname = Properties.Settings.Default.computername,
            os = getOs(), macprodid = macProdId();
            reqData = String.Format("action=createDvcRecord&usrid={0}&dvcname={1}&dvcos={2}&macprodid={3}",usrid,compname,os,macprodid);
            try {
                if (request.Onrequest(reqPage, reqData).Equals("1"))
                {
                    return true;
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Could not save settings");
                    return false;
                }
           }
           catch(Exception e){
               MessageBox.Show(e.ToString());
               return false;
           }
        }

        private bool updateDvcRecord()
        {
            String reqData = String.Format("action=setDvcname&usrid={0}&dvcid={1}&dvcname={2}", Properties.Settings.Default.uid, getDeviceId(), Properties.Settings.Default.computername);

            try
            {
                if (request.Onrequest(reqPage,reqData).Equals("1"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception e){
                MessageBox.Show(e.ToString());
                return false;

            }
        }

        private String macProdId()
        {
            String macprodId="";

            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();

            foreach (ManagementObject mo in moc)
            {
                if (mo["MacAddress"] != null)
                {
                    macprodId = mo["MacAddress"].ToString();
                }
            }

            ManagementObjectSearcher MOS = new ManagementObjectSearcher("Select * From Win32_BaseBoard");

            foreach (ManagementObject getserial in MOS.Get())
            {
                    macprodId += "_"+getserial["SerialNumber"].ToString();
            }
            Console.WriteLine(macprodId);
            return macprodId;
        }

        private String getOs()
        {
            String OS="";
            ManagementObjectSearcher mos = new ManagementObjectSearcher("select * from Win32_OperatingSystem");

            foreach (ManagementObject managementObject in mos.Get())
            {

                if (managementObject["Caption"] != null)
                {
                    OS = managementObject["Caption"].ToString();
                }
            }

           
            return OS;
        }

        public void unlinkDevice()
        {
            reqData = String.Format("action=unlinkDvc&usrid={0}&dvcid={1}", Properties.Settings.Default.uid,getDeviceId());
            Console.WriteLine(reqData);
            Console.WriteLine(request.Onrequest(reqPage,reqData));
        }

    }
}
