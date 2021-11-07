using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ASP.NETCoreWebApplication.Models
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
