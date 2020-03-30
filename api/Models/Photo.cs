using System;
namespace DatingApp.API.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsMain { get; set; }

        // ? adding these two props defines and tells Entity Framework the relationship between this table and the user table.
        // ? UserId will no longer be nullable and deleting a User will cause the photo to be deleted too.
        // ! however, we do not want these to be returned, so we need a DTO for Photo and an automapper registered accordingly.
        public User User { get; set; }
        public int UserId { get; set; }
    }
}