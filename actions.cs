using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Mobideskv2
{
    class actions
    {
        private String curAction
        {
            get;
            set;
        }
        static ftp ftp = new ftp();
        static httprequest request = new httprequest();

        public static void UploadFile(String fileFrom, String uploadTo)
        {
            try
            {
                if(ftp.upload(uploadTo,fileFrom)){
                    var settings = Properties.Settings.Default;
                    FileInfo fileInfo = new FileInfo(fileFrom);
                    String reqData = String.Format("action=addFile?usrid={0}&dir={1}&fileSize={2}",settings.uid,settings.directorypath,fileInfo.Length);
                    if (request.Onrequest("userFile.php", reqData).Equals("success"))
                    {
                        initset upsize = new initset();
                        upsize.updatesize(fileInfo.Length);
                    }
                }
            }catch(Exception e){

            }
        }

       

    }
}
