using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingWebApp.Infrastructure;

namespace ShoppingWebApp.Controllers
{
    public class ProductsController : Controller
    {

        private readonly ShoppingWebAppContext context;
        public ProductsController(ShoppingWebAppContext context)
        {
            this.context = context;
        }

        // GET /products
        public async Task<IActionResult> Index(int p = 1)
        {
            //add pagination
            int pageSize = 6;
            var products = context.Products.OrderByDescending(x => x.Id).Skip((p - 1) * pageSize).Take(pageSize);

            ViewBag.PageNumber = p;
            ViewBag.PageRange = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((decimal)context.Products.Count() / pageSize);

            return View(await products.ToListAsync());
        }
    }
}