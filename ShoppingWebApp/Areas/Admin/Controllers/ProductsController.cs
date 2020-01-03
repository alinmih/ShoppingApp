using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoppingWebApp.Infrastructure;
using ShoppingWebApp.Models;

namespace ShoppingWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {

        private readonly ShoppingWebAppContext context;
        private readonly IWebHostEnvironment webHostEnvironment;

        public ProductsController(ShoppingWebAppContext context, IWebHostEnvironment webHostEnvironment)
        {
            this.context = context;
            this.webHostEnvironment = webHostEnvironment;
        }


        // GET /admin/products/index
        public async Task<IActionResult> Index(int p=1)
        {
            //add pagination
            int pageSize = 6;
            var products = context.Products.OrderByDescending(x => x.Id).Include(x => x.Category).Skip((p-1) * pageSize).Take(pageSize);

            ViewBag.PageNumber = p;
            ViewBag.PageRange = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((decimal)context.Products.Count() / pageSize);

            return View(await products.ToListAsync());
        }

        // GET /admin/products/create
        public IActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(context.Categories.OrderBy(x => x.Sorting), "Id", "Name");

            return View();
        }

        // POST /admin/products/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            ViewBag.CategoryId = new SelectList(context.Categories.OrderBy(x => x.Sorting), "Id", "Name");

            //check if model is valid
            if (ModelState.IsValid)
            {
                //set the page prop
                product.Slug = product.Name.ToLower().Replace(" ", "-");

                //get the slug which is equal to current
                var slug = await context.Products.FirstOrDefaultAsync(x => x.Slug == product.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "The product allready exists");
                    return View(product);
                }

                //add the image file
                string imageName = "noimage.png";
                if (product.ImageUpload !=null)
                {
                    //set the directory using webHostEnvironment
                    string uploadsDir = Path.Combine(webHostEnvironment.WebRootPath, "media/products");
                    //set the name of the image to be unique
                    imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                    //set the full image path
                    string filePath = Path.Combine(uploadsDir, imageName);
                    
                    //upload using FileStream class
                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await product.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                }

                //set the property name
                product.Image = imageName;


                //add the page to db
                context.Add(product);
                await context.SaveChangesAsync();

                TempData["Success"] = "The product has been added!";

                return RedirectToAction("Index");
            }

            return View(product);
        }

    }
}