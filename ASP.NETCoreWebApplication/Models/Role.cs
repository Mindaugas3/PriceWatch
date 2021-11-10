using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ASP.NETCoreWebApplication.Models
{
    public class Role: IdentityRole
    {
        [Key]
        public string id { get; set; }

        public Role() : base()
        {
            
        }
    }
}