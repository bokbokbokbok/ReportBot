namespace ReportBot.Common.DTOs;

public class UserDTO
{
    public int Id { get; set; }
    public long ChatId { get; set; }
    public int WorksnapsId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
