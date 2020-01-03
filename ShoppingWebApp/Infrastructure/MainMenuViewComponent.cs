using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingWebApp.Infrastructure
{


    public class MainMenuViewComponent: ViewComponent
    {
        //get dependencies throw ctor
        private readonly ShoppingWebAppContext context;
        public MainMenuViewComponent(ShoppingWebAppContext context)
        {
            this.context = context;
        }

        //set a method to be invoked
        public async Task<IViewComponentResult> InvokeAsync()
        {
            //get the pages from database
            var pages = await GetPagesAsync();
            return View(pages);
        }

        //method to get the pages
        private Task<List<Page>> GetPagesAsync()
        {
            return context.Pages.OrderBy(x => x.Sorting).ToListAsync();
        }
    }
}
