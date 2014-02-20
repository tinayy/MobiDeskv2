using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace Mobideskv2
{
    class httprequest
    {
        /*
        private HttpWebRequest request;
        private HttpWebResponse response;
        private StreamReader reader;
        private Stream streamData;
        private String result;
        String httpurl = "http://localhost//mobidesk//_desk//";
        */
        public String Onrequest(String reqPage, String reqData)
        {
            String httpurl = "http://localhost//mobidesk//_desk//";
            String httpurl2 = "http://mobidesk.net//mobidesk//scs//_desk//";
            try
            {
                byte[] input = new ASCIIEncoding().GetBytes(reqData);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(httpurl + reqPage);
                request.ContentType = "application/x-www-form-urlencoded";
                request.Method = "POST";
                request.ContentLength = reqData.Length;
                Stream streamData = request.GetRequestStream();
                streamData.Write(input, 0, input.Length);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                String result = reader.ReadToEnd();
                reader.Close();
                response.Close();

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}