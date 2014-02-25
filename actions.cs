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

        public static void UploadFile(String uploadTo, String fileFrom)
        {
            try
            {
                    //fileFrom = fileFrom.Replace("\\","\\\\");
                    ftp.upload(uploadTo, fileFrom);
                    Console.WriteLine("Uploaded to fileserver");
                    Console.WriteLine("Add to db: " + fileFrom);
                    var settings = Properties.Settings.Default;
                    FileInfo fileInfo = new FileInfo(uploadTo);
                    String reqData = String.Format("action=addFile&usrid={0}&dir={1}&fileSize={2}",settings.uid,fileFrom,fileInfo.Length);
                   
                    String result = request.Onrequest("userFile.php", reqData);
                    if (result.Equals("success"))
                    {
                        Console.WriteLine("Successfully Added");
                        initset upsize = new initset();
                        upsize.updatesize(fileInfo.Length);
                    }
                    else
                    {
                        Console.WriteLine(result);
                    }
                
            }catch(Exception e){
                throw e;
            }
        }

       

    }
}
