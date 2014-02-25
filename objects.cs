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
            
            switch(ans){
                case "lts":
                    if (queue.Count() > 0)
                    {
                        Console.WriteLine("Start Queue. Count: " + queue.Count());
                        localToServer();
                    }
                break;
                case "stl":
                    if(queueFrmServer.Count() > 0)
                    {
                        Console.WriteLine("Start Queue. Count: " + queueFrmServer.Count());
                        serverToLocal();
                    }
                break;
            }

            if(!monitorChanges.isLocalMonitoringEnabled && !monitorChanges.isServerMonitoringEnabled){
               // monitorChanges.start_loc();
               // monitorChanges.start_srv();
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
                String check = newfilename != "" ? newfilename : filename;

                if (changetype.Equals("dlt"))
                {

                    Console.WriteLine("File to delete: " + withoutRoot);
                    withoutRoot = withoutRoot.Replace("\\","\\\\");
                    String reqData = String.Format("action=deleteFile&usrid={0}&dir={1}", uid, withoutRoot);
                    Console.WriteLine(request.Onrequest("userFile.php", reqData));
                }

                else if (ul.isFile(check))
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

                        
                            //updateSize.updatesize(-(getFilesize(withoutRoot)));
                            //deduct size
                            

                        case "rnm":
                            
                            FileInfo fileInf = new FileInfo(newfilename);
                            String newfile = localFiles.getfilewithoutroot(newfilename);
                            newfile = newfile.Replace("\\","\\\\");
                            String withoutRoot2 = "\\\\"+fileInf.Name;
                            withoutRoot = withoutRoot.Replace("\\","\\\\");
                            Console.WriteLine("File to rename: " + withoutRoot + "\nRename To: " + withoutRoot2);
                            
                                ftp.rename(withoutRoot2, withoutRoot);
                            
                                reqData = String.Format("action=renameFile&usrid={0}&dir={1}&renameTo={2}", uid, withoutRoot, newfile);
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
                            FileInfo fileInf = new FileInfo(newfilename);
                            String withoutRoot2 = fileInf.Name;
                            Console.WriteLine("Folder to rename: " + withoutRoot + "\nRename To: " + withoutRoot2);
                            String[] dirFiles = Directory.GetFiles(newfilename);
                            if(dirFiles.Count()>0){
                                Console.WriteLine("Folder have filesss!");
                                foreach(String f in dirFiles){                
                                    FileInfo fileName = new FileInfo(f);
                                    String oldDir = withoutRoot+"\\\\"+fileName.Name;
                                    String newDir = localFiles.getfilewithoutroot(newfilename)+"\\\\"+fileName.Name;
                                    Console.WriteLine("Old path: "+oldDir+" New Path: "+newDir);
                                    String reqData = String.Format("action=renameFile&usrid={0}&dir={1}&renameTo={2}&", uid, oldDir, newDir);
                                    Console.WriteLine(request.Onrequest("userFile.php", reqData));
                                }
                            }
                            ftp.rename(withoutRoot2, withoutRoot);
                            //check if folder has files
                            //get all files and rename path!
                            //String reqData = String.Format("action=renameFile&usrid={0}&dir={1}&renameTo={2}&", uid, withoutRoot, withoutRoot2);
                            //Console.WriteLine(request.Onrequest("userFile.php", reqData));
                            break;
                    }

                    if (!monitorChanges.isLocalMonitoringEnabled && !monitorChanges.isServerMonitoringEnabled)
                    {
                        monitorChanges.start_loc();
                        monitorChanges.start_srv();
                    }
                }


                
            }
            while (queue.Count() != 0);
        }


        private static void serverToLocal()
        {
            ftp ftp = new ftp();
            do{
                String[] q = queueFrmServer.Dequeue().Split(':');
                String changetype = q[0];
                String dir =  q[1];
                String oldDir = q[2];

                    Console.WriteLine(changetype);

                switch(changetype){
                    case "ctd":
                        String path = Path.Combine(Properties.Settings.Default.directorypath,dir);
                        ftp.download(dir,path);
                    break;

                    case "rnm":
                        oldDir =Properties.Settings.Default.directorypath +oldDir;
                        dir = Properties.Settings.Default.directorypath+ dir;
                        Console.WriteLine("Old dir: "+oldDir+" New dir: "+dir);
                        if(File.Exists(oldDir)){
                            Console.WriteLine("File exists. Rename file: "+oldDir+" To: "+dir);
                            File.Move(oldDir,dir);
                        }

                    break;

                    case "dlt":
                        dir = Properties.Settings.Default.directorypath + dir;
                        Console.WriteLine("Delete file: "+dir);
                        if(File.Exists(dir)){
                            Console.WriteLine("File exists. Delete file");
                            File.Delete(dir);
                        }
                    break;

                    case "updt":
                    break;
                }

            }
            while(queueFrmServer.Count() != 0);
        }

    }


   
}
