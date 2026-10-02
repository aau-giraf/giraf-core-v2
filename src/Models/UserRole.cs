using Microsoft.EntityFrameworkCore;


[PrimaryKey(nameof(UserId), nameof(Type))] //Congigure a composite key in order
public class UserRole
{
    public int UserId {get; init; }
    public required User User { get; init; }
    public RoleType Type { get; init; }
}

public enum RoleType
{
    Admin, Citizen, Teacher, Parent
}