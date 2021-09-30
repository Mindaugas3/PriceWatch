using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ASP.NETCoreWebApplication.Models
{
    public class Proxy
    {
        private IPAddress ip;
        private String username;
        private String password;
        private int port;
        Proxy(IPAddress ip, String username, String password, int port)
        {
            this.ip = ip;
            this.username = username;
            this.password = password;
            this.port = port;
        }

        //redirect traffic through proxy
        public HttpWebResponse Route(HttpWebRequest request, System.Net.WebHeaderCollection headers, string body = "", string method = "GET")
        {
            WebProxy myproxy;
            myproxy = new WebProxy(this.ip.ToString(), this.port);
            if (this.username == "")
            {
                myproxy.Credentials = new NetworkCredential(this.username, this.password);
            }
            myproxy.BypassProxyOnLocal = false;
            request.Proxy = myproxy;
            request.Method = method;
            request.Headers = headers;
            if (method == "POST")
            {
                var data = Encoding.ASCII.GetBytes(body);
                request.ContentLength = data.Length;
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            HttpWebResponse response = (HttpWebResponse) request.GetResponse();
            return response;
        }

    }
}