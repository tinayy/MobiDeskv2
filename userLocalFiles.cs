using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Windows.Forms;

namespace Mobideskv2
{
    class userLocalFiles
    {
        String localdirpath = Properties.Settings.Default.directorypath;
        bind _rcs = new bind();

        public void createlocalFolder()
        {
            if (localdirpath != null)
            {
                if (!Directory.Exists(localdirpath))
                {
                    try
                    {
                        Directory.CreateDirectory(localdirpath);
                    }
                    catch(Exception e){
                        System.Windows.Forms.MessageBox.Show(e.ToString());
                    }
                   
                }
            }
        }

        public String[] getDirectories()
        {
            String dir = Properties.Settings.Default.directorypath;
            try
            {                
                String[] subdirs = Directory.GetDirectories(dir, "*", SearchOption.AllDirectories);
                for (int cnt = 0; cnt != subdirs.Length;cnt++ )
                {
                    subdirs[cnt] = subdirs[cnt].Replace(dir,"");
                    
                }
                return subdirs;
            }
            catch(Exception e){
                throw e;
            }
        }

        public String[] getlocalfilesfullpath()
        {
            String dir = Properties.Settings.Default.directorypath;
            
            try
            {
                String[] allfiles = Directory.GetFiles(dir,"*",SearchOption.AllDirectories);              
                return allfiles;
            }
            catch(Exception e){
                throw e;
            }
        }

        public  String[] getlocalfileswithoutroot()
        {
            String dir = Properties.Settings.Default.directorypath;
            String[] files = getlocalfilesfullpath();
            List <string> nFiles = new List<string>();

            Console.WriteLine("GET LOCAL FILES");
            try
            {
                try
                {
                    if (Properties.Settings.Default.lastupdate != "")
                    {
                        Console.WriteLine("LAST UPDATE NOT EMPTY: " + Properties.Settings.Default.lastupdate);
                        foreach (String f in files)
                        {
                            
                            //Console.WriteLine(f);
                            FileInfo fileInf = new FileInfo(f);
                            DateTime CreationTime = fileInf.CreationTime;
                            DateTime LastWrite = fileInf.LastWriteTime;
                            DateTime lastUpdate = DateTime.ParseExact(Properties.Settings.Default.lastupdate, "MM/dd/yyyy HH:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture);
                            int compare = DateTime.Compare(lastUpdate, CreationTime);
                            Console.WriteLine("Compare result: "+compare);
                            if (compare < 0)
                            {
                                Console.WriteLine("Creation Time newer.Add to localFiles List: " + f);
                                nFiles.Add(f);
                            }
                            else if (compare > 0)
                            {
                                int lw = DateTime.Compare(lastUpdate, LastWrite);
                                if (lw < 0)
                                {
                                    Console.WriteLine("LastWrite Time newer.Add to localFiles List: " + f);
                                    nFiles.Add(f);
                                }
                                else if(lw > 0)
                                {
                                    Console.WriteLine("LastWrite Time older.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Same dateTime: " + f);
                            }
                            //nFiles.Add(f);
                        }
                    }
                    else
                    {
                        Console.WriteLine("No last update. Get all local files ");
                        foreach (String f in files)
                        {
                            nFiles.Add(f);
                        }
                    }
                    files = nFiles.ToArray();
                }
                finally { 
                    for (int cnt = 0; cnt != files.Length; cnt++)
                    {

                        files[cnt] = files[cnt].Replace(dir, "");
                        Console.WriteLine(files[cnt]);

                    }
                }
                return files;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public  String getfileDir(String f)
        {
            try
            {
                String fl = f.Replace(Properties.Settings.Default.directorypath,"");
                String filedironly = fl.Substring(0,fl.LastIndexOf('\\'));
                return filedironly;
            }
            catch(Exception e){
                throw e;
            }
        }

        public  String getfilewithoutroot(String f)
        {
            
            String file = f.Replace(Properties.Settings.Default.directorypath,"");
            return file;
        }

        public bool isFile(String path)
        {
            try
            {
                FileAttributes attr = File.GetAttributes(path);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                    return false;
                else
                    return true;
            }
            catch(Exception e){
                throw e;
            }
        }


    }
}
