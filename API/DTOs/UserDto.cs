namespace API.DTOs
{
    // this is what is returned when a user has successfully registered
    public class UserDto
    {
        public string UserName { get; set; }
        public string Token { get; set; }
    }
}