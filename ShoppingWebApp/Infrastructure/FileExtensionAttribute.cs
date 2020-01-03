using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingWebApp.Infrastructure
{
    public class FileExtensionAttribute :ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //// get the context for ShoppingWebAppContext
            //var context = (ShoppingWebAppContext)validationContext.GetService(typeof(ShoppingWebAppContext));

            var file = value as IFormFile;
            if (file!=null)
            {
                //get the extension from file
                var extension = Path.GetExtension(file.FileName);

                //set the allowed extensions
                string[] extensions = { ".jpg", ".png" };

                //check if file extension is in allowed extensions
                // if not return the result of GetErrorMessage method
                // else return success
                bool result = extensions.Any(x => extension.EndsWith(x));

                if (!result)
                {
                    return new ValidationResult(GetErrorMessage());
                }

            }
            
            return ValidationResult.Success;

        }

        private string GetErrorMessage()
        {
            return "Allowed extensions are .jpg and .png";
        }
    }
}
