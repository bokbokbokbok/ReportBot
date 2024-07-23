using McgTgBotNet.Attributes;

namespace McgTgBotNet.DTOs
{
    public class UserDTO
    {
        [XMLProperty("id")]
        public int Id { get; set; }
        [XMLProperty("login")]
        public string Login { get; set; }
        [XMLProperty("first_name")]
        public string FirstName { get; set; }
        [XMLProperty("last_name")]
        public string LastName { get; set; }
        [XMLProperty("email")]
        public string Email { get; set; }
        [XMLProperty("api_token")]
        public string ApiToken { get; set; }
    }
}
