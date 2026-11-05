namespace giraf_core_v2.DTOs.Images;

public class GenerateImageDTO
{
    public string Prompt { get; set; }
    public int MapId { get; set; }
    public required string TicketId { get; set; }
    public int TypeId { get; set; }
}