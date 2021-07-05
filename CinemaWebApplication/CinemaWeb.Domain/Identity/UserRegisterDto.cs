using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaWeb.Domain.Identity
{
    public class UserRegisterDto
    {
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirm Password is required")]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The Password and Confirm Password do not match.")]
        public string ConfirmPassword { get; set; }
        public string Role { get; set; }
    }
}
