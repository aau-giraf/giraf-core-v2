namespace giraf_core_v2.DTOs.Images;

public class AiGenerationRequestDTO
{
    public required string TicketId { get; set; }
    public required int UserId { get; set; }
    public required string Prompt { get; set; }
    public int? CitizenId { get; set; }
    public int? ClassId { get; set; }
    public int? OrganizationId { get; set; }
}