using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TrainingAssignment.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Enter Product Name.")]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required(ErrorMessage ="Please select product category.")]
        public string Category { get; set; }

        [Required(ErrorMessage ="Please enter product price.")]
        public double Price { get; set; }

        [Required(ErrorMessage ="Please select product quantity.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage ="Please enter product Short-description.")]
        [MaxLength(50)]
        public string ShortDesc { get; set; }

        [Required(ErrorMessage = "Please enter product Long-description.")]
        [MaxLength(150)]
        public string LongDesc { get; set; }

        [Required]
        [FileExtensions(Extensions ="jpg,jpeg",ErrorMessage ="Please select .jpg or .jpeg extension image.")]
        public string SmallImage { get; set; }

        [Required]
        [FileExtensions(Extensions = "jpg,jpeg", ErrorMessage = "Please select .jpg or .jpeg extension image.")]
        public string LargeImage { get; set; }

        public int User_Id { get; set; }

        public int totalRecords { get; set; }
    }
}