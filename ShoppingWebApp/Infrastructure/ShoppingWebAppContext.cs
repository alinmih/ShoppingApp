using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShoppingWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingWebApp.Infrastructure
{
    //updated to IdentityDbContext<AppUser> to use identity
    public class ShoppingWebAppContext : IdentityDbContext<AppUser>
    {
        public ShoppingWebAppContext(DbContextOptions<ShoppingWebAppContext> options) :base(options)
        {
        }


        public DbSet<Page> Pages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }



    }
}
