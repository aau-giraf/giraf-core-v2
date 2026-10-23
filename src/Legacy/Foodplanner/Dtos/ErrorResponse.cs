// Copied from foodplanner-api: FoodplannerModels/Account/Misc/ErrorResponse.cs (only the namespace changed)
// [JsonPropertyName] keeps "Message" and "Email" PascalCase in the JSON, exactly like the old backend
using System.Text.Json.Serialization;

namespace giraf_core_v2.Legacy.Foodplanner.Dtos;

// Class for errors in controllers
public class ErrorResponse
{
    [JsonPropertyName("Message")]
    public String[]? Message { get; set; }

    [JsonPropertyName("Email")]
    public String[]? Email { get; set; }
}