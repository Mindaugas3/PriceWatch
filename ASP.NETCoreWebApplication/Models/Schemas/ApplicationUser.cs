using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ASP.NETCoreWebApplication.Models.Schemas
{
    public class ApplicationUser : IdentityUser
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string pwd { get; set; }
        public string defaultCurrency { get; set; }

        public ApplicationUser()
        {
            
        }
    }
}
