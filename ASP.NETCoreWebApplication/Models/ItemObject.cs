using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ASP.NETCoreWebApplication.Models
{
    public class ItemObject
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int Source_id { get; set; }
        public string category { get; set; }
        [Required]
        public string title { get; set; }
        [Required]
        public string url { get; set; }
        [Required]
        public float price { get; set; }
        public float shipping { get; set; }
        public int shippingDuration { get; set; }
        public int Currency_id { get; set; }
        public string returns { get; set; }
        public float weight { get; set; }
        [Required]
        public string description { get; set; }
        public Photo img { get; set; }

        public ItemObject()
        {
            
        }
    }
}