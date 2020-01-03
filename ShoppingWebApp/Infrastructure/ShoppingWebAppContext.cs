using Microsoft.EntityFrameworkCore;
using ShoppingWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingWebApp.Infrastructure
{
    public class ShoppingWebAppContext : DbContext
    {
        public ShoppingWebAppContext(DbContextOptions<ShoppingWebAppContext> options) :base(options)
        {
        }


        public DbSet<Page> Pages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }



    }
}
