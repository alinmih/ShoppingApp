using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoppingWebApp.Infrastructure;
using ShoppingWebApp.Models;

namespace ShoppingWebApp.Areas.Admin.Controllers
{

    [Authorize(Roles = "admin")]
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

        // GET /admin/products/details/5
        public async Task<IActionResult> Details(int id)
        {
            //get the product from db
            Product product = await context.Products.Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
            {
                return NotFound();
            }
            //retur page detail 
            return View(product);
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


        // GET /admin/products/edit/5
        public async Task<IActionResult> Edit(int id)
        {
            Product product = await context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            ViewBag.CategoryId = new SelectList(context.Categories.OrderBy(x => x.Sorting), "Id", "Name", product.CategoryId);

            //retur product detail 
            return View(product);
        }

        // POST /admin/products/edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            ViewBag.CategoryId = new SelectList(context.Categories.OrderBy(x => x.Sorting), "Id", "Name", product.CategoryId);

            //check if model is valid
            if (ModelState.IsValid)
            {
                //set the page prop
                product.Slug = product.Name.ToLower().Replace(" ", "-");

                //get the slug which is equal to current
                var slug = await context.Products.Where(x=>x.Id != id).FirstOrDefaultAsync(x => x.Slug == product.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "The product allready exists");
                    return View(product);
                }

                //edit the image file
                if (product.ImageUpload != null)
                {
                    //set the directory using webHostEnvironment
                    string uploadsDir = Path.Combine(webHostEnvironment.WebRootPath, "media/products");

                    //remove the old image if exists one
                    if (!string.Equals(product.Image, "noimage.png"))
                    {
                        string oldImagePath = Path.Combine(uploadsDir, product.Image);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    //set the name of the image to be unique
                    string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                    //set the full image path
                    string filePath = Path.Combine(uploadsDir, imageName);

                    //upload using FileStream class
                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await product.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    //set the property name
                    product.Image = imageName;
                }


                //add the page to db
                context.Update(product);
                await context.SaveChangesAsync();

                TempData["Success"] = "The product has been edited!";

                return RedirectToAction("Index");
            }

            return View(product);
        }

        // GET /admin/products/delete/5
        public async Task<IActionResult> Delete(int id)
        {
            Product product = await context.Products.FindAsync(id);

            if (product == null)
            {
                TempData["Error"] = "The product does not exist!";

            }
            else
            {
                //set the directory using webHostEnvironment
                string uploadsDir = Path.Combine(webHostEnvironment.WebRootPath, "media/products");
                //remove the old image if exists one
                if (!string.Equals(product.Image, "noimage.png"))
                {
                    string oldImagePath = Path.Combine(uploadsDir, product.Image);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                context.Products.Remove(product);
                await context.SaveChangesAsync();
                TempData["Success"] = "The product has been deleted!";

            }
            //retur page detail 
            return RedirectToAction("Index");
        }




    }
}