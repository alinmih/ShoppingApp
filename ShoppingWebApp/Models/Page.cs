using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingWebApp.Models
{
    public class Page
    {
        public int Id { get; set; }
        [Required, MinLength(2, ErrorMessage ="Minimum lenght is 2")]
        public string Title { get; set; }
        public string Slug { get; set; }
        [Required, MinLength(4, ErrorMessage = "Minimum lenght is 4")]
        public string Content { get; set; }
        public int Sorting { get; set; }

    }
}
