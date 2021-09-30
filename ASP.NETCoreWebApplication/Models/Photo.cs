using System.Net;
using System.Runtime.InteropServices;

namespace ASP.NETCoreWebApplication.Models
{
    public class Photo
    {
        private IPAddress href;
        private int width;
        private int height;

        public Photo(IPAddress href, int width, int height)
        {
            this.href = href;
            this.width = width;
            this.height = height;
        }
    }
}