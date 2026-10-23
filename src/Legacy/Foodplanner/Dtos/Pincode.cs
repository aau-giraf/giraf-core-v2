// Copied from foodplanner-api: FoodplannerModels/Account/User/Pincode.cs (only the namespace changed)
namespace giraf_core_v2.Legacy.Foodplanner.Dtos;

// Pincode class
// Used in user controller to check or update pincode
public class Pincode
{
    public required string PinCode { get; set; }
}