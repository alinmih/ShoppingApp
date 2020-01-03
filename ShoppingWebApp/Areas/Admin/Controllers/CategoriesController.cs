using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingWebApp.Infrastructure;
using ShoppingWebApp.Models;

namespace ShoppingWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriesController : Controller
    {
        private readonly ShoppingWebAppContext context;
        public CategoriesController(ShoppingWebAppContext context)
        {
            this.context = context;
        }

        // GET /admin/categories/index
        public async Task<IActionResult> Index()
        {
            return View(await context.Categories.OrderBy(x=>x.Sorting).ToListAsync());
        }


        // GET /admin/categories/create
        public IActionResult Create() => View();

        // POST /admin/categories/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            //check if model is valid
            if (ModelState.IsValid)
            {
                //set the page prop
                category.Slug = category.Name.ToLower().Replace(" ", "-");
                category.Sorting = 100;

                //get the slug which is equal to current
                var slug = await context.Categories.FirstOrDefaultAsync(x => x.Slug == category.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "The category allready exists");
                    return View(category);
                }

                //add the page to db
                context.Add(category);
                await context.SaveChangesAsync();

                TempData["Success"] = "The category has been added!";


                return RedirectToAction("Index");
            }

            return View(category);
        }

        // GET /admin/categories/edit/5
        public async Task<IActionResult> Edit(int id)
        {
            Category category = await context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }
            //retur page detail 
            return View(category);
        }

        // POST /admin/categories/edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,Category category)
        {
            //check if model is valid
            if (ModelState.IsValid)
            {
                
                category.Slug = category.Name.ToLower().Replace(" ", "-");

                //search the category except the current and see if return something
                var slug = await context.Categories.Where(x => x.Id != category.Id).FirstOrDefaultAsync(x => x.Slug == category.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "The category allready exists");
                    return View(category);
                }

                //add the page to db
                context.Update(category);
                await context.SaveChangesAsync();

                TempData["Success"] = "The category has been edited!";


                return RedirectToAction("Edit", new { id });
            }

            return View(category);
        }

        // GET /admin/categories/delete/5
        public async Task<IActionResult> Delete(int id)
        {
            Category category = await context.Categories.FindAsync(id);

            if (category == null)
            {
                TempData["Error"] = "The category does not exist!";

            }
            else
            {
                context.Categories.Remove(category);
                await context.SaveChangesAsync();
                TempData["Success"] = "The category has been deleted!";
            }
            //retur page detail 
            return RedirectToAction("Index");
        }

        // POST /admin/categories/reorder
        [HttpPost]
        public async Task<IActionResult> Reorder(int[] id)
        {
            int count = 1;
            foreach (var categoryId in id)
            {
                Category category = await context.Categories.FindAsync(categoryId);
                category.Sorting = count;
                context.Update(category);
                await context.SaveChangesAsync();
                count++;
            }

            return Ok();
        }
    }
}