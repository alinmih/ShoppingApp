using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingWebApp.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string productName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get { return Quantity * Price; }}
        public string Image { get; set; }

        public CartItem()
        {
        }

        public CartItem(Product product)
        {
            ProductId = product.Id;
            productName = product.Name;
            Price = product.Price;
            Quantity = 1;
            Image = product.Image;
        }
    }
}
