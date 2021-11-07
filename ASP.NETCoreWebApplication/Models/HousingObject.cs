using System.ComponentModel.DataAnnotations;

namespace ASP.NETCoreWebApplication.Models
{
    public class HousingObject
    {
        [Key]
        public int Id { get; set; }
        [Required] public int Source_id { get; set; }
        [Required] public string title { get; set; }
        [Required] public string url { get; set; } //unique
        [Required] public float price { get; set; }
        [Required] public string location { get; set; }

        public long timestamp { get; set; }
        public string Currency { get; set; }
        
        public int rooms { get; set; }

        public int area { get; set; }
        [Required] public int floorsMax { get; set; }
        [Required] public int floorsThis { get; set; }
        [Required] public string description { get; set; }
        public string imgUrl { get; set; }

        public HousingObject()
        {
            
        }
    }
}