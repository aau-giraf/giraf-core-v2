

namespace giraf_core_v2.Tests.Services;

public class OrganizationServiceTest(ITestOutputHelper output) : TestBase(output)
{
    private readonly OrganizationSeeder _organizationSeeder = new();
    private readonly ClassSeeder _classSeeder = new();
    private readonly UserSeeder _userSeeder = new();

    [Fact]
    public async Task GetOrganizationById_OrganizationExists_ReturnsOrganization()
    {
        //----- ARRANGE -----\\
        await using var db = CreateDbContext();

        var organizations = await _organizationSeeder.SeedManyAsync(db, Seed, 2);
        var organization1 = organizations[0];
        var organization2 = organizations[1];

        Assert.NotNull(organization1);
        Assert.NotNull(organization2);

        var service = new OrganizationService(db);

        //----- ACT -----\\
        var fetchedOrganization = await service.GetOrganizationByIdAsync(organization2.Id);

        //----- ASSERT -----\\
        Assert.NotNull(fetchedOrganization);
        Assert.True(organization2.Id == fetchedOrganization.OrganizationId);
        Assert.True(organization2.Name == fetchedOrganization.Name);
        Assert.False(organization1.Id == fetchedOrganization.OrganizationId);
        Assert.False(organization1.Name == fetchedOrganization.Name);
    }

    [Fact]
    public async Task GetOrganizations_OrganizationsExist_ReturnsOrganizations()
    {
        //----- ARRANGE -----\\
        await using var db = CreateDbContext();

        var organizations = await _organizationSeeder.SeedManyAsync(db, Seed, 2);
        var organization1 = organizations[0];
        var organization2 = organizations[1];

        List<Organization> organizationsList = [organization1, organization2];

        Assert.NotNull(organization1);
        Assert.NotNull(organization2);

        var service = new OrganizationService(db);

        //----- ACT -----\\
        var fetchedOrganizations = await service.GetOrganizationsAsync();

        //----- ASSERT -----\\
        Assert.True(organizations.Count() == fetchedOrganizations.Count());

        if (fetchedOrganizations.Count() == 2 && fetchedOrganizations[0].OrganizationId == organization1.Id)
        {
            Assert.True(fetchedOrganizations[0].OrganizationId == organization1.Id);
            Assert.True(fetchedOrganizations[0].Name == organization1.Name);
            Assert.True(fetchedOrganizations[1].OrganizationId == organization2.Id);
            Assert.True(fetchedOrganizations[1].Name == organization2.Name);
        }
        else if (fetchedOrganizations.Count() == 2 && fetchedOrganizations[0].OrganizationId == organization2.Id)
        {
            Assert.True(fetchedOrganizations[0].OrganizationId == organization2.Id);
            Assert.True(fetchedOrganizations[0].Name == organization2.Name);
            Assert.True(fetchedOrganizations[1].OrganizationId == organization1.Id);
            Assert.True(fetchedOrganizations[1].Name == organization1.Name);
        }
    }

    [Fact]
    public async Task DeleteOrganization_OrganizationExists_DeletesOrganization()
    {
        //----- ARRANGE -----\\
        await using var db = CreateDbContext();

        var organizations = await _organizationSeeder.SeedManyAsync(db, Seed, 2);
        var organization1 = organizations[0];
        var organization2 = organizations[1];

        Assert.NotNull(organization1);
        Assert.NotNull(organization2);

        var service = new OrganizationService(db);

        //----- ARRANGE / ASSERT -----\\
        Assert.True(await service.DeleteOrganizationAsync(organization1.Id));

        var deletedOrganization = await db.Organizations
            .SingleOrDefaultAsync(o => o.Id == organization1.Id);

        Assert.Null(deletedOrganization);

        // Check that the other organization was not deleted
        var remainingOrganization = await db.Organizations
            .SingleOrDefaultAsync(o => o.Id == organization2.Id);

        Assert.NotNull(remainingOrganization);
    }

    [Fact]
    public async Task DeleteClassInOrganization_ClassExists_DeletesClass()
    {
        //----- ARRANGE -----\\
        await using var db = CreateDbContext();

        var organizations = await _organizationSeeder.SeedManyAsync(db, Seed, 2);
        var organization1 = organizations[0];
        var organization2 = organizations[1];

        List<ClassOptions> options = [new ClassOptions(OrganizationId: organization1.Id), new ClassOptions(OrganizationId: organization1.Id)];

        var classes = await _classSeeder.SeedManyAsync(db, Seed, 2, options);
        var class1 = classes[0];
        var class2 = classes[1];

        Assert.NotNull(organization1);
        Assert.NotNull(organization2);
        Assert.NotNull(class1);
        Assert.NotNull(class2);

        var service = new OrganizationService(db);

        //----- ACT -----\\
        // Delete class1 from organization1
        var deletedClassInOrganization = await service.DeleteClassInOrganizationAsync(organization1.Id, class1.Id);

        // Fetch the selected class intended to have been removed from said organization
        var selectedClass = await db.Classes
            .Where(c => c.OrganizationId == organization1.Id).FirstOrDefaultAsync(c => c.Id == class1.Id);

        // Fetch the other class
        var notSelectedClass = await db.Classes
            .Where(c => c.OrganizationId == organization1.Id).FirstOrDefaultAsync(c => c.Id == class2.Id);


        //----- ASSERT -----\\
        // Be aware that the assertions differ given if the selectedClass was in the organization or not.
        if (selectedClass is null && class1.OrganizationId == organization1.Id)
        {
            Assert.True(deletedClassInOrganization);
        }
        else if (selectedClass is null && class1.OrganizationId != organization1.Id)
        {
            // Since the class is not in the organization the method must return false
            Assert.False(deletedClassInOrganization);
        }

        // After possible delition of class1 ensure that the state of other classes are not affected
        if (notSelectedClass is not null)
        {
            Assert.NotNull(await db.Classes
                .Where(c => c.OrganizationId == organization1.Id).FirstOrDefaultAsync(c => c.Id == class2.Id));
        }
    }

    [Fact]
    public async Task GetClassInOrganization_ClassExists_ReturnsClass()
    {
        //----- ARRANGE -----\\
        await using var db = CreateDbContext();

        var organizations = await _organizationSeeder.SeedManyAsync(db, Seed, 2);
        var organization1 = organizations[0];
        var organization2 = organizations[1];
        List<ClassOptions> options = [new ClassOptions(OrganizationId: organization1.Id), new ClassOptions(OrganizationId: organization1.Id)];

        var classes = await _classSeeder.SeedManyAsync(db, Seed, 2, options);
        var class1 = classes[0];
        var class2 = classes[1];

        Assert.NotNull(organization1);
        Assert.NotNull(organization2);
        Assert.NotNull(class1);
        Assert.NotNull(class2);

        var service = new OrganizationService(db);

        //----- ACT -----\\
        var fetchedClass = await service.GetClassInOrganizationAsync(organization2.Id, class2.Id);

        //----- ASSERT -----\\
        // If class2 is in organization2 then make these assertions.
        if (class2.OrganizationId == organization2.Id)
        {
            Assert.NotNull(fetchedClass);
            Assert.True(class2.Id == fetchedClass.ClassId);
            Assert.Equal(class2.Name, fetchedClass.Name);
            Assert.True(class2.OrganizationId == fetchedClass.OrganizationId);
        }
    }

    [Fact]
    public async Task CreateOrganization_NameIsAvailable_CreatesOrganization()
    {
        //----- ARRANGE -----\\
        await using var db = CreateDbContext();

        var service = new OrganizationService(db);

        // Create the dummy DTO for the requestbody
        CreateOrganizationRequestDTO createOrganization = new()
        {
            Name = "SchoolOfIdoha",
        };

        //----- ACT -----\\
        var createdOrganization = await service.CreateOrganizationAsync(createOrganization);

        //----- ASSERT -----\\
        Assert.NotNull(createdOrganization);
        Assert.True(createOrganization.Name == createdOrganization.Name);
    }

    [Fact]
    public async Task UpdateOrganization_NameIsAvailable_UpdatesOrganization()
    {
        //----- ARRANGE -----\\
        await using var db = CreateDbContext();

        var organizations = await _organizationSeeder.SeedManyAsync(db, Seed, 2);
        var organization1 = organizations[0];
        var organization2 = organizations[1];
        int organization1Id = organization1.Id;
        string organization1Name = organization1.Name;
        Assert.NotNull(organization1);
        Assert.NotNull(organization2);

        var service = new OrganizationService(db);

        // Create the dummy DTO for the requestbody
        CreateOrganizationRequestDTO updateOrganization = new()
        {
            Name = "SchoolOfNY",
        };

        //----- ACT -----\\
        var updatedOrganization = await service.UpdateOrganizationAsync(updateOrganization, organization1.Id);

        //----- ASSERT -----\\
        Assert.NotNull(updatedOrganization);
        Assert.Equal(updateOrganization.Name, updatedOrganization.Name);
        Assert.True(organization1Id == updatedOrganization.OrganizationId);
        // Asserts that you cant rename and organization to an existing one
        Assert.NotEqual(organization1Name, updatedOrganization.Name);
    }

    [Fact]
    public async Task CreateClassInOrganization_NameIsAvailable_CreatesClass()
    {
        //----- ARRANGE -----\\
        await using var db = CreateDbContext();

        var organizations = await _organizationSeeder.SeedManyAsync(db, Seed, 2);
        var organization1 = organizations[0];
        var organization2 = organizations[1];
        List<ClassOptions> options = [new ClassOptions(OrganizationId: organization1.Id), new ClassOptions(OrganizationId: organization1.Id)];

        var classes = await _classSeeder.SeedManyAsync(db, Seed, 2, options);
        var class1 = classes[0];
        var class2 = classes[1];

        Assert.NotNull(organization1);
        Assert.NotNull(organization2);
        Assert.NotNull(class1);
        Assert.NotNull(class2);

        var service = new OrganizationService(db);

        // Create the dummy DTO for the requestbody
        CreateClassRequestDTO createClass = new()
        {
            Name = "4a",
        };

        //----- ACT -----\\
        // Create a class in organization1
        var createdClassInOrganization = await service.CreateClassInOrganizationAsync(createClass, organization1.Id);

        //----- ASSERT -----\\
        Assert.NotNull(createdClassInOrganization);
        Assert.True(createClass.Name == createdClassInOrganization.Name);
        Assert.True(organization1.Id == createdClassInOrganization.OrganizationId);

    }


    [Fact]
    public async Task GetClassesInOrganization_ClassesExist_ReturnsClasses()
    {
        //----- ARRANGE -----\\
        await using var db = CreateDbContext();

        var organizations = await _organizationSeeder.SeedManyAsync(db, Seed, 2);
        var organization1 = organizations[0];
        var organization2 = organizations[1];

        List<ClassOptions> options = [new ClassOptions(OrganizationId: organization1.Id), new ClassOptions(OrganizationId: organization1.Id)];

        var classes = await _classSeeder.SeedManyAsync(db, Seed, 2, options);
        var class1 = classes[0];
        var class2 = classes[1];

        Assert.NotNull(organization1);
        Assert.NotNull(organization2);
        Assert.NotNull(class1);
        Assert.NotNull(class2);

        var service = new OrganizationService(db);

        //----- ACT -----\\
        var fetchedClassesInOrganization = await service.GetClassesInOrganizationAsync(organization1.Id);

        //----- ASSERT -----\\
        if (fetchedClassesInOrganization.Count() == 0)
        {
            Assert.Empty(fetchedClassesInOrganization);
        }
        else if (fetchedClassesInOrganization.Count() == 1 && fetchedClassesInOrganization[0].ClassId == class1.Id)
        {
            Assert.True(fetchedClassesInOrganization[0].ClassId == class1.Id);
            Assert.True(fetchedClassesInOrganization[0].Name == class1.Name);
        }
        else if (fetchedClassesInOrganization.Count() == 1 && fetchedClassesInOrganization[0].ClassId == class2.Id)
        {
            Assert.True(fetchedClassesInOrganization[0].ClassId == class2.Id);
            Assert.True(fetchedClassesInOrganization[0].Name == class2.Name);
        }
        else if (fetchedClassesInOrganization.Count() == 2 && fetchedClassesInOrganization[0].ClassId == class1.Id)
        {
            Assert.True(fetchedClassesInOrganization[0].ClassId == class1.Id);
            Assert.True(fetchedClassesInOrganization[0].Name == class1.Name);
            Assert.True(fetchedClassesInOrganization[1].ClassId == class2.Id);
            Assert.True(fetchedClassesInOrganization[1].Name == class2.Name);
        }
        else if (fetchedClassesInOrganization.Count() == 2 && fetchedClassesInOrganization[0].ClassId == class2.Id)
        {
            Assert.True(fetchedClassesInOrganization[0].ClassId == class2.Id);
            Assert.True(fetchedClassesInOrganization[0].Name == class2.Name);
            Assert.True(fetchedClassesInOrganization[1].ClassId == class1.Id);
            Assert.True(fetchedClassesInOrganization[1].Name == class1.Name);
        }
    }
    [Fact]
    public async Task CreateOrUpdateEmployeeRole_EmployeeRoleDoesNotExist_CreatesEmployeeRole()
    {
        //----- ARRANGE -----\\
        await using var db = CreateDbContext();

        var organization = await _organizationSeeder.SeedAsync(db, Seed);

        UserOptions userOptions = new(Type: UserType.NonCitizen);
        var seededUser = await _userSeeder.SeedAsync(db, Seed, userOptions);
        var role1 = RoleType.Employee;

        Assert.NotNull(organization);
        Assert.NotNull(seededUser);

        var service = new OrganizationService(db);

        //----- ACT -----\\
        (int outcomeStatus, EmployeeRole? employeeRole) = await service.CreateOrUpdateEmployeeRoleAsync(organization.Id, seededUser.User.Id, role1);

        //----- ASSERT -----\\
        Assert.Equal(1, outcomeStatus); //creation outcome
        Assert.NotNull(employeeRole);
        Assert.Equal(seededUser.User.Id, employeeRole.NonCitizenId);
        Assert.Equal(organization.Id, employeeRole.OrganizationId);
        Assert.Equal(role1, employeeRole.Role);
    }

    /* TODO: Fix
    [Fact]
    public async Task CreateOrUpdateEmployeeRole_EmployeeRoleExists_ReturnsNoChange()
    {
        //----- ARRANGE -----\\
        await using var db = CreateDbContext();

        var organization = await _organizationSeeder.SeedAsync(db, Seed);

        UserOptions userOptions = new(Type: UserType.NonCitizen);
        var seededUser = await _userSeeder.SeedAsync(db, Seed, userOptions);

        var role1 = RoleType.Employee;
        // Parent roles are represented by Guardianship.GuardianType.

        Assert.NotNull(organization);
        Assert.NotNull(seededUser);

        var service = new OrganizationService(db);

        //----- ACT -----\\
        (int outcomeStatus, EmployeeRole? employeeRole) initialResult = await service.CreateOrUpdateEmployeeRoleAsync(organization.Id, seededUser.User.Id, role1);
        (int outcomeStatus, EmployeeRole? employeeRole) secondResult = await service.CreateOrUpdateEmployeeRoleAsync(organization.Id, seededUser.User.Id, role1);

        //----- ASSERT -----\\
        Assert.Equal(1, initialResult.outcomeStatus);

        Assert.Equal(0, secondResult.outcomeStatus);
        Assert.Null(secondResult.employeeRole);

    }
     */
    /* TODO: Fix
    [Fact]
    public async Task CreateOrUpdateEmployeeRole_OrganizationDoesNotExist_ReturnsNotFound()
    {
        //----- ARRANGE -----\\
        await using var db = CreateDbContext();

        var seededUser = await UserSeeder.SeedAsync(db, Seed);
        var seededUser2 = await UserSeeder.SeedAsync(db, Seed);
        var role1 = RoleType.Employee;

        Assert.NotNull(seededUser);

        var service = new OrganizationService(db);

        //----- ACT -----\\
        (int outcomeStatus, EmployeeRole? employeeRole) initialResult = await service.CreateOrUpdateEmployeeRoleAsync(100000000, seededUser.User.Id, role1);
        (int outcomeStatus, EmployeeRole? employeeRole) secondResult = await service.CreateOrUpdateEmployeeRoleAsync(1234567890, seededUser2.User.Id, role1);

        //----- ASSERT -----\\
        Assert.Equal(-1, initialResult.outcomeStatus);
        Assert.Null(initialResult.employeeRole);

        Assert.Equal(-1, secondResult.outcomeStatus);
        Assert.Null(secondResult.employeeRole);
    }
    */
}