using McgTgBotNet.Attributes;

namespace McgTgBotNet.DTOs
{
    public class UserDTO
    {
        [XMLProperty("id")]
        public int Id { get; set; }
        [XMLProperty("login")]
        public string Login { get; set; } = string.Empty;
        [XMLProperty("first_name")]
        public string FirstName { get; set; } = string.Empty;
        [XMLProperty("last_name")]
        public string LastName { get; set; } = string.Empty;
        [XMLProperty("email")]
        public string Email { get; set; } = string.Empty;
        [XMLProperty("api_token")]
        public string ApiToken { get; set; } = string.Empty;
    }
}
