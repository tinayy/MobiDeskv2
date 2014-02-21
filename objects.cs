using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Mobideskv2
{
    class objects
    {
        public String stat { get; set; }
        public String file {get; set;}
        public static Queue<string> queue = new Queue<string>();
        public static Queue<string> queueFrmServer = new Queue<string>();
        
        
        public static void processQueue(String ans)
        {          
            Console.WriteLine("Start Queue. Count: "+queue.Count());
            switch(ans){
                case "lts":
                    if (queue.Count() > 0)
                    {
                        localToServer();
                    }
                break;
                case "stl":
                    if(queueFrmServer.Count() > 0)
                    {
                        serverToLocal();
                    }
                break;
            }
            
            
        }

        private static long getFilesize(String fpath)
        {
            FileInfo fileinfo = new FileInfo(fpath);
            long size = fileinfo.Length;
            return size;
        }

        private static void localToServer()
        {
            userLocalFiles localFiles = new userLocalFiles();
            ftp ftp = new ftp();
            httprequest request = new httprequest();
            initset updateSize = new initset();
            userLocalFiles ul = new userLocalFiles();
            do
            {
                Console.WriteLine("Local to server changes");
                String[] q = queue.Dequeue().Split('?');
                String changetype = q[0];
                String filename = q[1];
                String newfilename = q[2];
                filename = filename.Replace("\\\\", "\\");
                newfilename = newfilename.Replace("\\\\", "\\");
                String withoutRoot = localFiles.getfilewithoutroot(filename);

                String uid = Properties.Settings.Default.uid;
                Console.WriteLine("Changetype: " + changetype);
                

                if (ul.isFile(newfilename != "" ? newfilename : filename))
                {
                    Console.WriteLine("It is file: " + withoutRoot);
                   
                    switch (changetype)
                    {

                        case "ctd":
                           
                            Console.WriteLine(Properties.Settings.Default.directorypath);
                            Console.WriteLine("File to Create: " + withoutRoot);
                            String reqData = String.Format("action=addFile&usrid={0}&dir={1}&fileSize={2}", uid, withoutRoot, getFilesize(withoutRoot));
                            filename = filename.Replace("\\", "\\\\");
                            if (ftp.upload(filename, withoutRoot))
                            {
                                Console.WriteLine(request.Onrequest("userFile.php", reqData));
                            }
                            break;

                        case "dlt":
                            
                            Console.WriteLine("File to delete: " + withoutRoot);
                            reqData = String.Format("action=deleteFile&usrid={0}&dir={1}", uid, withoutRoot);
                            Console.WriteLine(request.Onrequest("userFile.php", reqData));
                            updateSize.updatesize(-(getFilesize(withoutRoot)));
                            //deduct size
                            break;

                        case "rnm":
                            String withoutRoot2 = localFiles.getfilewithoutroot(newfilename).Replace("\\","\\\\");
                            withoutRoot = withoutRoot.Replace("\\","\\\\");
                            Console.WriteLine("File to rename: " + withoutRoot + "\nRename To: " + withoutRoot2);
                            
                                ftp.rename(withoutRoot2, withoutRoot);
                            
                                reqData = String.Format("action=renameFile&usrid={0}&dir={1}&renameTo={2}", uid, withoutRoot, withoutRoot2);
                                Console.WriteLine(request.Onrequest("userFile.php", reqData));
                            
                           
                            break;

                        case "chg":
                            filename = filename.Replace("\\", "\\\\");
                            //ftp.upload(filename, withoutRoot);
                            break;
                    }
                    //overwrite existing with ftp
                    //set filesize to db
                }
                else
                {
                    Console.WriteLine("It is dir: " + withoutRoot);
                    switch (changetype)
                    {

                        case "ctd":

                            //Console.WriteLine(Properties.Settings.Default.directorypath);
                            Console.WriteLine("Folder to Create: " + withoutRoot);
                            filename = filename.Replace("\\", "\\\\");
                            ftp.createFolder(withoutRoot);

                            break;

                        case "dlt":

                            Console.WriteLine("Folder to delete: " + withoutRoot);
                            ftp.deletedir(withoutRoot);

                            break;

                        case "rnm":
                            String withoutRoot2 = localFiles.getfilewithoutroot(newfilename);
                            Console.WriteLine("Folder to rename: " + withoutRoot + "\nRename To: " + withoutRoot2);
                            ftp.rename(withoutRoot2, withoutRoot);
                            String reqData = String.Format("action=renameFile&usrid={0}&dir={1}&renameTo={2}&", uid, withoutRoot, withoutRoot2);
                            Console.WriteLine(request.Onrequest("userFile.php", reqData));
                            break;
                    }
                }


            }
            while (queue.Count() != 0);
        }


        private static void serverToLocal()
        {
            do{
                String[] q = queueFrmServer.Dequeue().Split('?');
                String changetype = q[0];
                String dir =  q[1];
                String oldDir = q[2];

                switch(changetype){
                    case "ctd":

                    break;

                    case "rnm":
                    break;

                    case "dlt":
                    break;

                    case "updt":
                    break;
                }

            }
            while(queueFrmServer.Count() != 0);
        }

    }


   
}
