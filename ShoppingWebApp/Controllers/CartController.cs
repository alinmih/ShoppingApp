using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShoppingWebApp.Infrastructure;
using ShoppingWebApp.Models;

namespace ShoppingWebApp.Controllers
{
    public class CartController : Controller
    {
        private readonly ShoppingWebAppContext context;
        public CartController(ShoppingWebAppContext context)
        {
            this.context = context;
        }

        //get /cart
        public IActionResult Index()
        {
            //get the list with items from session or create one empty list
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            CartViewModel cartVM = new CartViewModel
            {
                CartItems = cart,
                GrandTotal = cart.Sum(x => x.Price * x.Quantity)
            };

            return View(cartVM);
        }

        //get /cart/add/5
        public async Task<IActionResult> Add(int id)
        {
            Product product = await context.Products.FindAsync(id);
            
            //get the list from session
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            //get the cartitem which is equal to product
            CartItem cartItem = cart.Where(x => x.ProductId == id).FirstOrDefault();
            
            //add new item in art, if exists increase the qty by one
            if (cartItem==null)
            {
                cart.Add(new CartItem(product));
            }
            else
            {
                cartItem.Quantity += 1;
            }

            //set session
            HttpContext.Session.SetJson("Cart", cart);


            if (HttpContext.Request.Headers["X-Requested-With"]!="XMLHttpRequest")
            {
                //redirect
                return RedirectToAction("Index");
            }

            return ViewComponent("SmallCart");
            

        }

        //get /cart/decrease/5
        public IActionResult Decrease(int id)
        {

            //get the list from session
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart");

            //get the cartitem which is equal to product
            CartItem cartItem = cart.Where(x => x.ProductId == id).FirstOrDefault();

            //add new item in art, if exists increase the qty by one
            if (cartItem.Quantity>1)
            {
                --cartItem.Quantity;
            }
            else
            {
                cart.RemoveAll(x => x.ProductId == id);
            }


            if (cart.Count==0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                //set session
                HttpContext.Session.SetJson("Cart", cart);
            }

            //redirect
            return RedirectToAction("Index");
        }

        //get /cart/remove/5
        public IActionResult Remove(int id)
        {

            //get the list from session
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart");
         
            cart.RemoveAll(x => x.ProductId == id);
 
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                //set session
                HttpContext.Session.SetJson("Cart", cart);
            }

            //redirect
            return RedirectToAction("Index");
        }

        //get /cart/clear
        public IActionResult Clear()
        {

            HttpContext.Session.Remove("Cart");

            if (HttpContext.Request.Headers["X-Requested-With"] != "XMLHttpRequest")
            {
                //redirect
                return Redirect(Request.Headers["Referer"].ToString());
            }

            //redirect
            //return RedirectToAction("Page","Pages");
            //return Redirect("/");
            return Ok();
        }

    }
}