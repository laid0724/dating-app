using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDto
    {
        // 400 error request will be thrown when these validations are not passed
        [Required] // validation
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}