

namespace ASP.NETCoreWebApplication.Models
{
    public class DataSource
    {
        public int id { get; set; }
        public int interval { get; set; }
        public string name { get; set; }
        public string href { get; set; }
        public int proxy_id { get; set; }
        public long last_scrapped { get; set; }

        public DataSource()
        {
            
        }
    }
}