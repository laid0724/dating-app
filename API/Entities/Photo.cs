using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    [Table("Photos")] // this ensures that the table is named as Tables in the db
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }
        public string PublicId { get; set; }

        /*
            this fully defines the relationship between this entity and AppUser,
            a one-to-many relationship.

            the effect that this does is that it:
                - makes this field non-nullable (AppUserId)
                - on delete will no longer be in "restrict", and "cascade" instead:
                    - restrict: deleting user will not delete the photos associated with the entity
                    - cascade: delete the photos in the ICollection as well.
        */
        
        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}