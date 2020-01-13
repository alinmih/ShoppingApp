using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingWebApp.Infrastructure;
using ShoppingWebApp.Models;

namespace ShoppingWebApp.Controllers
{
    [Authorize]
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

        // GET /products/category
        public async Task<IActionResult> ProductsByCategory(string categorySlug, int p = 1)
        {
            //get all categories
            Category category = await context.Categories.Where(x => x.Slug == categorySlug).FirstOrDefaultAsync();
            if (category == null)
            {
                return RedirectToAction("Index");
            }

            //add pagination
            int pageSize = 6;
            //get products in function of categories
            var products = context.Products.OrderByDescending(x => x.Id)
                                            .Where(x => x.CategoryId == category.Id)
                                            .Skip((p - 1) * pageSize).Take(pageSize);

            ViewBag.PageNumber = p;
            ViewBag.PageRange = pageSize;

            ViewBag.TotalPages = (int)Math.Ceiling((decimal)context.Products.Where(x => x.CategoryId == category.Id).Count() / pageSize);

            ViewBag.CategoryName = category.Name;
            ViewBag.CategorySlug = categorySlug;

            return View(await products.ToListAsync());
        }
    }
}