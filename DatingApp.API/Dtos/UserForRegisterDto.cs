using System;
using System.ComponentModel.DataAnnotations;
using DatingApp.API.Models;

namespace DatingApp.API.Dtos
{
    public class UserForRegisterDto
    {
        // ? this makes this param required for API calls:
        [Required]
        public string Username { get; set; }

        [Required]
        // ? you can also set validation with custom error messages:
        [StringLength(8, MinimumLength = 4, ErrorMessage = "The password must be between 4 and 8 characters.")]
        public string Password { get; set; }

    }
}