using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Globalization;

namespace Mobideskv2
{
    class sync
    {
        ftp ftp = new ftp();
        objects obj = new objects();
        log log = new log();
        httprequest request = new httprequest();
        userServerFiles serverFiles = new userServerFiles();
        userLocalFiles localFiles = new userLocalFiles();
       
        

        private decimal frsize()
        {
                int m = Properties.Settings.Default.maxsize;
                decimal tsize = 1073741824UL * (ulong)m;
                decimal fsize = tsize - Properties.Settings.Default.totalb;
                Console.WriteLine("Max size: "+tsize+"\nFree size: "+fsize);
                return fsize;         
        }
        
        public string update(String ans)
        {
            System.Windows.Forms.MessageBox.Show("Get Files: "+ans);
            
            //String[] serverfilesToDelete = serverFiles.getserverFilesToDelete();

            Console.WriteLine("Last Update: "+Properties.Settings.Default.lastupdate);

            //deleteFiles(serverfilesToDelete);

            if( ans== "0"){
                    Console.WriteLine("Sync");
                    String[] localfiles = localFiles.getlocalfileswithoutroot();
                    String[] serverfiles = serverFiles.getserverFiles();
                    syncOn(localfiles, serverfiles);
                    return "done";
                    //sync
            }
            else if(ans =="1"){
                     Console.WriteLine("\n---UploadStart---");
                     String[] localfiles = localFiles.getlocalfileswithoutroot();
                    uploadtoServer(localfiles);
                    Console.WriteLine("---UploadEnd---\n");
                    return "done";
            }
            else if (ans == "2")
            {
                String[] serverfiles = serverFiles.getserverFiles();
                downloadtoLocal(serverfiles);
                Console.WriteLine("---DownloadEnd---\n");
                    return "done";
            }
            else if (ans == "3")
            {
                Console.WriteLine("Process Queue");
                Console.WriteLine("---UpdateStart---\n");
                objects.processQueue("lts");
                Console.WriteLine("---UpdateEnd---\n");
                return "done";
            }
                    
                    return "";

            
        }

        private void syncOn(String[] localfiles, String[] serverfiles)
        {
            
            if(localfiles.Length>0 && serverfiles.Length>0){

                String[] toUpload = localfiles.Except(serverfiles).ToArray();
                String[] toDownload = serverfiles.Except(localfiles).ToArray();
                System.Windows.Forms.MessageBox.Show("Upload some");
                uploadtoServer(toUpload);
                System.Windows.Forms.MessageBox.Show("Download some");
                downloadtoLocal(toDownload);
            }
            else if(localfiles.Length>0 && serverfiles.Length==0){
                System.Windows.Forms.MessageBox.Show("Upload all");
                uploadtoServer(localfiles);
                //upload all
            }
            else if(localfiles.Length==0 && serverfiles.Length>0){
                System.Windows.Forms.MessageBox.Show("Download all");
                downloadtoLocal(serverfiles);
                //download all
            }
            else{
                // do nothing
            }
            //get localfiles
            //get serverfiles
            //compare
            //check size
            //rename if exists (Copy from *date*)
            //upload complete - add to total size
        }

        private void uploadtoServer(String[] files)
        {
            try
            {
                if(files.Length>0){
                    Console.WriteLine("Array file length: " + files.Length);
                   
                    //Console.WriteLine("Object status: "+bind.status);
                    foreach(String f in files){
                        
                       
                        //obj.file = f;
                        String fl = Properties.Settings.Default.directorypath+"\\"+f ;
                        Console.WriteLine("File to Upload: " + fl);
                        //create directory -> create file                      
                        if (localFiles.getfileDir(fl)!="")
                        {
                            Console.WriteLine("Create Folder :" + localFiles.getfileDir(fl));
                            ftp.createFolder(localFiles.getfileDir(fl));
                        }         
                        FileInfo fileinfo = new FileInfo(fl);
                        long size = fileinfo.Length;
                        Console.WriteLine("File size: " + size);
                        if(size<frsize()){
                            String file = f.Replace("\\", "\\\\");
                            Console.WriteLine("Upload: " + fl + "\nTo:" + f);
                            actions.UploadFile(fl,file);
                        }
                        else
                        {
                            Console.WriteLine("File exceeds maximum capacity");
                        }                        
                    }
                }

               
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private bool uploadtoDb(String file, decimal size)
        {
            try
            {
                String uid = Properties.Settings.Default.uid;
                Console.WriteLine("Dir to db: " + file);
                String reqData = String.Format("action=addFile&usrid={0}&dir={1}&fileSize={2}",uid,file,size);
                Console.WriteLine(request.Onrequest("userFile.php",reqData));
                return true;
            }
            catch(Exception e){
                return false;
                throw e;
            }
           
        }

        public void downloadtoLocal(String[] files)
        {
            try
            {
                Console.WriteLine("\n--Download Start--");
                Console.WriteLine("Download Array :" + files.Length);
                
                
                foreach (String f in files)
                {
                    obj.file = f;
                    Console.WriteLine("Download :" + f);
                    if (f.Contains("\\"))
                    {
                        
                        if (localFiles.getfileDir(f)!=null || localFiles.getfileDir(f)!="")
                        {
                            String dir = Properties.Settings.Default.directorypath + "\\" + localFiles.getfileDir(f);
                        if (!Directory.Exists(dir))
                        {
                             
                            Console.WriteLine("Create Directory :" + dir);
                            Directory.CreateDirectory(dir);
                        }
                        }
                        String p = Properties.Settings.Default.directorypath+"\\"+f;
                       
                        ftp.download(f, p);

                    }
                }
                Console.WriteLine("--Download Finish--\n");
            }

            catch (Exception e)
            {
                throw e;
            }
        }


        public void date()
        {
            
            DateTime time = DateTime.Now;             // Use current time
            string format = "MM/dd/yyyy HH:mm:ss tt";            // Use this format - 00/00/0000 24:00:00 am
            Console.WriteLine("Date finished sync: "+time.ToString(format));
            Properties.Settings.Default.lastupdate = time.ToString(format);
            Properties.Settings.Default.Save();

        }

        private void deleteFiles(String[] serverFiles)
        {
            String date = Properties.Settings.Default.lastupdate.ToString();
            //System.Windows.Forms.MessageBox.Show(date);
            DateTime lastUpdate = DateTime.ParseExact(date, "MM/dd/yyyy HH:mm:ss tt", CultureInfo.InvariantCulture);
            foreach(String f in serverFiles){
                String path = Path.Combine(Properties.Settings.Default.directorypath,f);
                if(File.Exists(path)){
                    FileInfo fileInf = new FileInfo(path);
                    DateTime lastWrite = fileInf.LastWriteTime;
                    DateTime creationDT = File.GetCreationTime(path);
                    int compare = DateTime.Compare(lastUpdate,creationDT);

                    if(compare < 0 ){
                        File.Delete(path);

                    }
                    
                }
            }
        }
        

    }
}
