namespace API.Entities
{
    // join table for many-to-many relationships
    // both table will need ICollection property (in this case, one table of AppUser)
    public class UserLike
    {
        public AppUser SourceUser { get; set; } // the user that likes someone
        public int SourceUserId { get; set; }
        public AppUser LikedUser { get; set; } // the user being liked
        public int LikedUserId { get; set; }
    }
}