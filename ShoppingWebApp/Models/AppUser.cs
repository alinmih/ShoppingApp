using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingWebApp.Models
{
    //class creation for creating tables with user
    public class AppUser :IdentityUser
    {
        public string Occupation { get; set; }
    }
}
