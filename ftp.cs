using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace Mobideskv2
{
    class ftp
    {

        private FtpWebRequest ftprequest;
        private FtpWebResponse ftpresponse;
        private Stream ftpstream;
        private int buffersize = 2048;

        private String host = ConnectionStrings.host + Properties.Settings.Default.uid;
        private String uname=ConnectionStrings.uname, pass=ConnectionStrings.pword;
        

        public void download(String toDownload, String downloadPath )
        {
            
            try
            {
                Console.WriteLine(String.Format("Ftp now--File from: {0}{1}",host,toDownload));
                ftprequest = (FtpWebRequest)FtpWebRequest.Create(host+toDownload);
                ftprequest.Credentials = new NetworkCredential(uname,pass);
                ftprequest.UseBinary = true;
                ftprequest.UsePassive = true;
                ftprequest.KeepAlive = true;
                ftprequest.Method = WebRequestMethods.Ftp.DownloadFile;
                ftpresponse = (FtpWebResponse)ftprequest.GetResponse();
                ftpstream = ftpresponse.GetResponseStream();
                Console.WriteLine(String.Format("Download to: {0}", downloadPath));
                FileStream localStream = new FileStream(downloadPath, FileMode.Create);

                byte[] byteBuffer = new byte[buffersize];
                int bytesRead = ftpstream.Read(byteBuffer, 0, buffersize);

                try
                {
                    while (bytesRead > 0)
                    {
                        localStream.Write(byteBuffer, 0, bytesRead);
                        bytesRead = ftpstream.Read(byteBuffer, 0, 2048);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }

                localStream.Close();
                ftpstream.Close();
                ftpresponse.Close();
                ftprequest = null;

            }
            catch(Exception e){
                
            }
        }

        public bool upload(String toUpload, String uploadPath)
        {
            FileInfo fileInfo = new FileInfo(toUpload);

            try
            {
                Console.WriteLine("Uid " + Properties.Settings.Default.uid);
                Console.WriteLine("Upload to: " + host + uploadPath);
                ftprequest = (FtpWebRequest)WebRequest.Create(host + uploadPath);
                ftprequest.Method = WebRequestMethods.Ftp.UploadFile;
                ftprequest.Credentials = new NetworkCredential(uname, pass);
                ftprequest.UsePassive = true;
                ftprequest.UseBinary = true;
                ftprequest.KeepAlive = true;

                ftprequest.ContentLength = fileInfo.Length;
                //ftprequest.Timeout = 60000;
                Console.WriteLine("File from: " + toUpload);

                int buffLength = 2048;
                byte[] buff = new byte[buffLength];

                // Opens a file stream (System.IO.FileStream) to read the file to be uploaded
               FileStream FileStream = fileInfo.OpenRead();
                try
                {
                    // Stream to which the file to be upload is written
                    ftpstream = ftprequest.GetRequestStream();

                    // Read from the file stream 2kb at a time
                    int contentLen = FileStream.Read(buff, 0, buffLength);
                    // Till Stream content ends

                    while (contentLen != 0)
                    {
                        // Write Content from the file stream to the FTP Upload Stream
                        ftpstream.Write(buff, 0, contentLen);
                        contentLen = FileStream.Read(buff, 0, buffLength);
                    }
                    ftpstream.Close();
                    ftprequest = null;
                    return true;
                }
                catch(Exception e){
                    return false;
                }
            }catch(Exception e){
                return false;
            }
        }

        public void createFolder(String folder)
        {
          
                try
                {
                    Console.WriteLine("Uid: " + Properties.Settings.Default.uid);
                    Console.WriteLine("Create Directory: "+host+folder);
                    ftprequest = (FtpWebRequest)FtpWebRequest.Create(host+folder);
                    ftprequest.Credentials = new NetworkCredential(uname,pass);
                    ftprequest.Method = WebRequestMethods.Ftp.MakeDirectory;
                    ftprequest.UsePassive = true;
                    ftprequest.UseBinary = true;
                    ftprequest.KeepAlive = true;
                    ftpresponse = (FtpWebResponse)ftprequest.GetResponse();
                    ftpresponse.Close();
                    ftprequest = null;             
                }
                catch (Exception e)
                {
                    //throw e;
                }
             
        }

        public void deletefile(String filename)
        {
            try
            {
                ftprequest = (FtpWebRequest)FtpWebRequest.Create(host+filename);
                ftprequest.Credentials = new NetworkCredential(uname,pass);
                ftprequest.UseBinary = true;
                ftprequest.UsePassive = true;
                ftprequest.KeepAlive = true;
                ftprequest.Method = WebRequestMethods.Ftp.DeleteFile;
                ftpresponse = (FtpWebResponse)ftprequest.GetResponse();
                ftpresponse.Close();
                ftprequest = null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void deletedir(String dir)
        {
            try
            {
                ftprequest = (FtpWebRequest)FtpWebRequest.Create(host+dir);
                ftprequest.Credentials = new NetworkCredential(uname, pass);
                ftprequest.UseBinary = true;
                ftprequest.UsePassive = true;
                ftprequest.KeepAlive = true;
                ftprequest.Method = WebRequestMethods.Ftp.RemoveDirectory;
                ftpresponse = (FtpWebResponse)ftprequest.GetResponse();
                ftpresponse.Close();
                ftprequest = null;
            }
            catch(Exception e){
                throw e;
            }
        }

        public void rename(String newfname, String origfname)
        {
            try
            {
                ftprequest = (FtpWebRequest)FtpWebRequest.Create(host+origfname);
                ftprequest.Credentials = new NetworkCredential(uname, pass);
                ftprequest.UsePassive = true;
                ftprequest.UseBinary = true;
                ftprequest.KeepAlive = true;
                ftprequest.Method = WebRequestMethods.Ftp.Rename;
                ftprequest.RenameTo = newfname;
                ftpresponse = (FtpWebResponse)ftprequest.GetResponse();
                ftpresponse.Close();
                ftprequest = null;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
                
            }
        }
    }
}
