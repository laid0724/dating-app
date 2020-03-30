using System;
using System.Collections.Generic;
using DatingApp.API.Models;

namespace DatingApp.API.Dtos
{
    public class UserForDetailedDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; } // ? if the property in the DTO is not in the original model, you need to configure AutoMapper accordingly.
        public string KnownAs { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PhotoUrl { get; set; }  // ? if the property in the DTO is not in the original model, you need to configure AutoMapper accordingly.
        public ICollection<PhotosForDetailedDto> Photos { get; set; }
    }
}