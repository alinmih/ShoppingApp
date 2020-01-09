using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingWebApp.Infrastructure
{
    public class CategoriesViewComponent: ViewComponent
    {
        //get dependencies throw ctor
        private readonly ShoppingWebAppContext context;
        public CategoriesViewComponent(ShoppingWebAppContext context)
        {
            this.context = context;
        }

        //set a method to be invoked
        public async Task<IViewComponentResult> InvokeAsync()
        {
            //get the pages from database
            var categories = await GetCategoriesAsync();
            return View(categories);
        }

        //method to get the pages
        private Task<List<Category>> GetCategoriesAsync()
        {
            return context.Categories.OrderBy(x => x.Sorting).ToListAsync();
        }
    }
}
