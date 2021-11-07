using System.ComponentModel.DataAnnotations;

namespace ASP.NETCoreWebApplication.Models
{
    public class LogEntry
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public int User_id { get; set; }

        public LogEntry()
        {
            
        }
    }
}