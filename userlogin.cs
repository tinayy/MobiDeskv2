using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Mobideskv2
{
    class userlogin
    {
        
        String reqPage = "usrLogin.php";
        ftp ftp = new ftp();
        httprequest request = new httprequest();

        public bool verifyuser(string usr_email, string usr_pword)
        {
            String reqData = "action=login&eadd="+usr_email+"&pword="+usr_pword;        
            String result = request.Onrequest(reqPage,reqData);
            
            if(result!=""){
               
                getUserDetails(result);
                
                return true;
            }
            else
            {
                return false;
            }
             
        }

        private void getUserDetails(String uid)
        {
            String reqData = "action=getDetails&usrid="+uid;
            String result = request.Onrequest(reqPage, reqData);
            
            String trimmed = result.Substring(1, result.Length - 2);
            System.Windows.Forms.MessageBox.Show(trimmed);
            initset settings = new initset();
            userServerFiles server = new userServerFiles();
            settings.settings("setInitProperties",trimmed);
            
            
            //create srverfolder

        }

    }
}
