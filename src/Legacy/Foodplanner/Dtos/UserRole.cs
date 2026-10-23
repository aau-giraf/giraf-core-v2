// Copied from foodplanner-api: FoodplannerModels/Account/User/User.cs (only the enum, and only the namespace changed).
// v2 also has a class called UserRole (giraf_core_v2.Models). In an adapter that needs both, add an alias:
// using FoodplannerUserRole = giraf_core_v2.Legacy.Foodplanner.Dtos.UserRole;
namespace giraf_core_v2.Legacy.Foodplanner.Dtos;

[Flags]
public enum UserRole
{
    Admin = 1 << 0,    // 1
    Child = 1 << 1,    // 2
    Teacher = 1 << 2,    // 4
    Parent = 1 << 3     // 8
}