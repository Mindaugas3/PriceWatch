using System.ComponentModel.DataAnnotations;

namespace ASP.NETCoreWebApplication.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string name { get; set; }

        public Category()
        {
            
        }
    }
}