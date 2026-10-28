namespace giraf_core_v2.Data;

public record CreateNonCitizenDTO
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}