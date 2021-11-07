using System.ComponentModel.DataAnnotations;

namespace ASP.NETCoreWebApplication.Models
{
    public class SearchHistory
    {


        [Key]
        public int Id { get; set; }
        public ItemObject item { get; set; }
        public HousingObject housingItem { get; set; }
        [Required]
        public int User_id { get; set; }
        [Required]
        public long timestamp { get; set; }
        
        public SearchHistory()
        {
            
        }
    }
}