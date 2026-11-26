namespace giraf_core_v2.DTOs.Images;

public enum ImageStatus { Pending, Done };

public class IsImageDoneResponseDTO
{
    public required ImageStatus Status { get; set; }
    public required string TicketId { get; set; }
    public int? ImageId { get; set; }
    public string? TitleEn { get; set; }
    public string? TitleDa { get; set; }

}