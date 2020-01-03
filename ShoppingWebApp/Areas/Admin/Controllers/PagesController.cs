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
    public class PagesController : Controller
    {
        private readonly ShoppingWebAppContext context;
        public PagesController(ShoppingWebAppContext context)
        {
            this.context = context;
        }

        // GET /admin/pages
        public async Task<IActionResult> Index()
        {
            //linq query to get all the pages
            IQueryable<Page> pages = from p in context.Pages orderby p.Sorting select p;
            
            //get al the pages from db
            List<Page> pagesList = await pages.ToListAsync();

            //ViewBag.Fruits = "Apples";

            //retur pages list
            return View(pagesList);
        }
        
        // GET /admin/pages/details/5
        public async Task<IActionResult> Details(int id)
        {
            Page page = await context.Pages.FirstOrDefaultAsync(x => x.Id == id);

            if (page == null)
            {
                return NotFound();
            }
            //retur page detail 
            return View(page);
        }

        // GET /admin/pages/create
        public IActionResult Create()
        {
            return View();
        }

        // POST /admin/pages/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Page page)
        {
            //check if model is valid
            if (ModelState.IsValid)
            {
                //set the page prop
                page.Slug = page.Title.ToLower().Replace(" ", "-");
                page.Sorting = 100;

                //get the slug which is equal to current
                var slug = await context.Pages.FirstOrDefaultAsync(x => x.Slug == page.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "The page allready exists");
                    return View(page);
                }

                //add the page to db
                context.Add(page);
                await context.SaveChangesAsync();

                TempData["Success"] = "The page has been added!";


                return RedirectToAction("Index");
            }

            return View(page);            
        }

        // GET /admin/page/edit/5
        public async Task<IActionResult> Edit(int id)
        {
            Page page = await context.Pages.FindAsync(id);

            if (page == null)
            {
                return NotFound();
            }
            //retur page detail 
            return View(page);
        }


        // POST /admin/pages/edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Page page)
        {
            //check if model is valid
            if (ModelState.IsValid)
            {
                //check if edited page is home, return slug='home' else set the new slug according
                page.Slug= page.Id == 1 ? "home": page.Slug = page.Title.ToLower().Replace(" ", "-");

                //search the slug except the current and see if return something
                var slug = await context.Pages.Where(x=>x.Id != page.Id).FirstOrDefaultAsync(x => x.Slug == page.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "The page allready exists");
                    return View(page);
                }

                //add the page to db
                context.Update(page);
                await context.SaveChangesAsync();

                TempData["Success"] = "The page has been edited!";


                return RedirectToAction("Edit", new {id=page.Id});
            }

            return View(page);
        }

        // GET /admin/page/delete/5
        public async Task<IActionResult> Delete(int id)
        {
            Page page = await context.Pages.FindAsync(id);

            if (page == null)
            {
                TempData["Error"] = "The page does not exist!";

            }
            else
            {
                context.Pages.Remove(page);
                await context.SaveChangesAsync();
                TempData["Success"] = "The page has been deleted!";
            }
            //retur page detail 
            return RedirectToAction("Index");
        }


        // POST /admin/pages/reorder
        [HttpPost]
        public async Task<IActionResult> Reorder(int[] id)
        {
            int count = 1;
            foreach (var pageId in id)
            {
                Page page = await context.Pages.FindAsync(pageId);
                page.Sorting = count;
                context.Update(page);
                await context.SaveChangesAsync();
                count++;
            }

            return Ok();
        }
    }
}