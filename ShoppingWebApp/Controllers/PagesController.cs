using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingWebApp.Infrastructure;
using ShoppingWebApp.Models;

namespace ShoppingWebApp.Controllers
{
    public class PagesController : Controller
    {
        private readonly ShoppingWebAppContext context;
        public PagesController(ShoppingWebAppContext context)
        {
            this.context = context;
        }

        // GET on / or /slug
        public async Task<IActionResult> Page(string slug)
        {
            if (slug == null)
            {
                return View(await context.Pages.Where(x => x.Slug == "home").FirstOrDefaultAsync());
            }

            Page page = await context.Pages.Where(x => x.Slug == slug).FirstOrDefaultAsync();

            if (page==null)
            {
                return NotFound();
            }

            return View(page);
        }
    }
}