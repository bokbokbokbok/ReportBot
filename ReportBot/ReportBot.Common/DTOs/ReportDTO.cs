using McgTgBotNet.DTOs;
using ReportBot.Common.DTOs.Project;

namespace ReportBot.Common.DTOs;

public class ReportDTO
{
    public int Id { get; set; }
    public DateTime Created { get; set; }
    public string Message { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime DateOfShift { get; set; }
    public long ChatId { get; set; }
    public UserDTO User { get; set; } = null!;
    public ProjectDTO Project { get; set; } = null!;
}