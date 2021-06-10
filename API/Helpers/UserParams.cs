namespace API.Helpers
{
    public class UserParams : PagedRequest
    {
        public string CurrentUserName { get; set; }
        public string Gender { get; set; }
    }
}