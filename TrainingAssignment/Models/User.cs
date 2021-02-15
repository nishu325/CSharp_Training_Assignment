using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TrainingAssignment.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Enter your name.")]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required(ErrorMessage ="Enter your Email-id.")]
        [MaxLength(50)]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}")]
        public string Email { get; set; }

        [Required(ErrorMessage ="Enter your Password.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,15}$")]
        public string Password { get; set; }
        
    }
}