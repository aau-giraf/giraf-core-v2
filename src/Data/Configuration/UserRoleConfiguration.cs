namespace giraf_core_v2.Data.Configuration;
using giraf_core_v2.Models;
public class UserRoleConfiguration {

    public static void Seed(AppDbContext context) {
        
        if (!context.Set<UserRole>().Any()) {

            User[] users = context.Set<User>().ToArray();

            var userRoles = new[] { 
                new UserRole{ UserId = users[0].Id, User = users[0], Type = RoleType.Citizen },
                new UserRole{ UserId = users[1].Id, User = users[1], Type = RoleType.Citizen },
                new UserRole{ UserId = users[2].Id, User = users[2], Type = RoleType.Citizen },
                new UserRole{ UserId = users[3].Id, User = users[3], Type = RoleType.Teacher },
                new UserRole{ UserId = users[4].Id, User = users[4], Type = RoleType.Parent },
                new UserRole{ UserId = users[5].Id, User = users[5], Type = RoleType.Admin },
            };

            context.Set<UserRole>().AddRange(userRoles);
            context.SaveChanges();
        }
    }

}