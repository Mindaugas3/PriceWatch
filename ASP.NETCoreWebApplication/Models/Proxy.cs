using System.ComponentModel.DataAnnotations;

namespace ASP.NETCoreWebApplication.Models
{
    public class Proxy
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string hostname { get; set; }
        public int port { get; set; }
        public string username { get; set; }
        public string proxypwd { get; set; }

        public Proxy()
        {
            
        }
    }
}