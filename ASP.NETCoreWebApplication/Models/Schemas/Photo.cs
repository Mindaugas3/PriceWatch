using System.ComponentModel.DataAnnotations;

namespace ASP.NETCoreWebApplication.Models.Schemas
{
    public class Photo
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string href { get; set; }
        public int Item_id { get; set; }
        public int size_w { get; set; }
        public int size_h { get; set; }

        public Photo()
        {
            
        }
    }
}