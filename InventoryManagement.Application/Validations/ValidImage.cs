using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace InventoryManagement.Application.Validations
{

    // for training i will use fluent validation 
    public static class ValidImage
    {
        static readonly List<string> Allows = new List<string>()
        {
            ".jpg" , 
            ".png" ,
        }; 
        public static ValidationResult? SureValidImage(IFormFile image)
        {
            if (image == null)
                return new ValidationResult("Image is required");
            if (Allows.Any(a => a == Path.GetExtension(image.FileName).ToLower()))
                return ValidationResult.Success;
            return new ValidationResult("allows types .jpg , .png"); 
        }
    }
}
