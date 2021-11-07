using System.ComponentModel.DataAnnotations;

namespace ASP.NETCoreWebApplication.Models
{
    // 1 1(view items) 1(Mindaugas) 1(Enabled)
    public class PermsList
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int Perm_id { get; set; }
        public int User_id { get; set; }
        public int Value { get; set; }

        public PermsList()
        {
            
        }
    }
}