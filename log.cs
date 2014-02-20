using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Mobideskv2
{
    public class log
    {
        public void writeLog(String data){
            DateTime saveNow = DateTime.Now;
            DateTime myDt = DateTime.SpecifyKind(saveNow, DateTimeKind.Local);

            string path = @"C:\Users\Kristina\log.txt";
            //File.AppendAllLines(path, new [] { data });
            File.AppendAllText(path, "\n" + myDt + "\t" + data + "\n");
        }
    }
}
