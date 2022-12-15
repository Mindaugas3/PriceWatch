using System.ComponentModel.DataAnnotations;

namespace ASP.NETCoreWebApplication.Models.Schemas
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string name { get; set; }
        public int sourceId { get; set; }
        public string href { get; set; }

        public Category()
        {
            
        }
    }
}