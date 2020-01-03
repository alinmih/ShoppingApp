using Microsoft.AspNetCore.Http;
using ShoppingWebApp.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingWebApp.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required, MinLength(2, ErrorMessage = "Minimum lenght is 2")]
        public string Name { get; set; }
        public string Slug { get; set; }

        [Required, MinLength(4, ErrorMessage = "Minimum lenght is 4")]
        public string Description { get; set; }

        [Column(TypeName ="decimal(18,2)")]
        public decimal Price { get; set; }

        //display name
        [Display(Name = "Category")]
        //validation of category
        [Range(1,int.MaxValue,ErrorMessage ="You must choose a category.")]
        public int CategoryId { get; set; }


        public string Image { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        //not put the property in database table
        [NotMapped]
        //add custom validation class
        [FileExtension]
        public IFormFile ImageUpload { get; set; }
    }
}
