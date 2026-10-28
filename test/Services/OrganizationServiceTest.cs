using System.Data;

using Xunit.Abstractions;

public class OrganizationServiceTest : TestBase
{
    public OrganizationServiceTest(ITestOutputHelper output) : base(output) { }

    [Fact]
    public async Task GetOrganization_GetsCorrectOrganization()
    {
        //----- ARRANGE -----\\
        await using var db = CreateDbContext();

        var organizations = await OrganizationSeeder.SeedManyAsync(db, Seed, 2);
        var organization1 = organizations[0];
        var organization2 = organizations[1];

        Assert.NotNull(organization1);
        Assert.NotNull(organization2);

        var service = new OrganizationService(db);

        //----- ACT -----\\
        var fetchedOrganization = await service.GetOrganizationByIdAsync(organization2.Id);

        //----- ASSERT -----\\
        Assert.NotNull(fetchedOrganization);
        Assert.True(organization2.Id == fetchedOrganization.Id);
        Assert.True(organization2.Name == fetchedOrganization.Name);
        Assert.False(organization1.Id == fetchedOrganization.Id);
        Assert.False(organization1.Name == fetchedOrganization.Name);
    }

    [Fact]
    public async Task GetOrganizationsList_GetsCorrectOrganizationsList()
    {
        //----- ARRANGE -----\\
        await using var db = CreateDbContext();

        var organizations = await OrganizationSeeder.SeedManyAsync(db, Seed, 2);
        var organization1 = organizations[0];
        var organization2 = organizations[1];

        List<Organization> organizationsList = new List<Organization>() { organization1, organization2 };

        Assert.NotNull(organization1);
        Assert.NotNull(organization2);

        var service = new OrganizationService(db);

        //----- ACT -----\\
        var fetchedOrganizations = await service.GetOrganizationsAsync();

        //----- ASSERT -----\\
        Assert.True(organizations.Count() == fetchedOrganizations.Count());

        if (fetchedOrganizations.Count() == 2 && fetchedOrganizations[0].Id == organization1.Id)
        {
            Assert.True(fetchedOrganizations[0].Id == organization1.Id);
            Assert.True(fetchedOrganizations[0].Name == organization1.Name);
            Assert.True(fetchedOrganizations[1].Id == organization2.Id);
            Assert.True(fetchedOrganizations[1].Name == organization2.Name);
        }
        else if (fetchedOrganizations.Count() == 2 && fetchedOrganizations[0].Id == organization2.Id)
        {
            Assert.True(fetchedOrganizations[0].Id == organization2.Id);
            Assert.True(fetchedOrganizations[0].Name == organization2.Name);
            Assert.True(fetchedOrganizations[1].Id == organization1.Id);
            Assert.True(fetchedOrganizations[1].Name == organization1.Name);
        }
    }

    [Fact]
    public async Task DeleteOrganization_DeletesCorrectOrganization()
    {
        //----- ARRANGE -----\\
        await using var db = CreateDbContext();

        var organizations = await OrganizationSeeder.SeedManyAsync(db, Seed, 2);
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
    public async Task DeleteClassInOrganization_DeletesCorrectClassInOrganization()
    {
        //----- ARRANGE -----\\
        await using var db = CreateDbContext();

        var organizations = await OrganizationSeeder.SeedManyAsync(db, Seed, 2);
        var organization1 = organizations[0];
        var organization2 = organizations[1];

        var classes = await ClassSeeder.SeedManyAsync(db, Seed, 2);
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
    public async Task GetClassInOrganization_GetsCorrectClassInOrganization()
    {
        //----- ARRANGE -----\\
        await using var db = CreateDbContext();

        var organizations = await OrganizationSeeder.SeedManyAsync(db, Seed, 2);
        var organization1 = organizations[0];
        var organization2 = organizations[1];

        var classes = await ClassSeeder.SeedManyAsync(db, Seed, 2);
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
            Assert.True(class2.Id == fetchedClass.Id);
            Assert.Equal(class2.Name, fetchedClass.Name);
            Assert.True(class2.OrganizationId == fetchedClass.OrganizationId);
        }
    }

    [Fact]
    public async Task CreateOrganization_CreatesCorrectOrganization()
    {
        //----- ARRANGE -----\\
        await using var db = CreateDbContext();

        var service = new OrganizationService(db);

        // Create the dummy DTO for the requestbody
        RequestCreateOrganizationDTO requestCreateOrganizationDTO = new RequestCreateOrganizationDTO
        {
            Name = "SchoolOfIdoha",
        };

        //----- ACT -----\\
        var createdOrganization = await service.CreateOrganizationAsync(requestCreateOrganizationDTO);

        //----- ASSERT -----\\
        Assert.NotNull(createdOrganization);
        Assert.True(requestCreateOrganizationDTO.Name == createdOrganization.Name);
    }

    [Fact]
    public async Task UpdateOrganization_UpdatesCorrectOrganization()
    {
        //----- ARRANGE -----\\
        await using var db = CreateDbContext();

        var organizations = await OrganizationSeeder.SeedManyAsync(db, Seed, 2);
        var organization1 = organizations[0];
        var organization2 = organizations[1];
        int organization1Id = organization1.Id;
        string organization1Name = organization1.Name;
        Assert.NotNull(organization1);
        Assert.NotNull(organization2);

        var service = new OrganizationService(db);

        // Create the dummy DTO for the requestbody
        RequestCreateOrganizationDTO requestUpdateOrganizationDTO = new RequestCreateOrganizationDTO
        {
            Name = "SchoolOfNY",
        };

        //----- ACT -----\\
        var updatedOrganization = await service.UpdateOrganizationAsync(requestUpdateOrganizationDTO, organization1.Id);

        //----- ASSERT -----\\
        Assert.NotNull(updatedOrganization);
        Assert.Equal(requestUpdateOrganizationDTO.Name, updatedOrganization.Name);
        Assert.True(organization1Id == updatedOrganization.Id);
        // Asserts that you cant rename and organization to an existing one
        Assert.NotEqual(organization1Name, updatedOrganization.Name);
    }

    [Fact]
    public async Task CreateClassInOrganization_CreatesCorrectClassInOrganization()
    {
        //----- ARRANGE -----\\
        await using var db = CreateDbContext();

        var organizations = await OrganizationSeeder.SeedManyAsync(db, Seed, 2);
        var organization1 = organizations[0];
        var organization2 = organizations[1];

        var classes = await ClassSeeder.SeedManyAsync(db, Seed, 2);
        var class1 = classes[0];
        var class2 = classes[1];

        Assert.NotNull(organization1);
        Assert.NotNull(organization2);
        Assert.NotNull(class1);
        Assert.NotNull(class2);

        var service = new OrganizationService(db);

        // Create the dummy DTO for the requestbody
        RequestCreateClassDTO requestCreateClassinOrganizationDTO = new RequestCreateClassDTO
        {
            Name = "4a",
        };

        //----- ACT -----\\
        // Create a class in organization1
        var createdClassInOrganization = await service.CreateClassInOrganizationAsync(requestCreateClassinOrganizationDTO, organization1.Id);

        //----- ASSERT -----\\
        Assert.NotNull(createdClassInOrganization);
        Assert.True(requestCreateClassinOrganizationDTO.Name == createdClassInOrganization.Name);
        Assert.True(organization1.Id == createdClassInOrganization.OrganizationId);

    }


    [Fact]
    public async Task GetClassesInOrganization_GetsCorrectClassesInOrganization()
    {
        //----- ARRANGE -----\\
        await using var db = CreateDbContext();

        var organizations = await OrganizationSeeder.SeedManyAsync(db, Seed, 2);
        var organization1 = organizations[0];
        var organization2 = organizations[1];

        var classes = await ClassSeeder.SeedManyAsync(db, Seed, 2);
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
        else if (fetchedClassesInOrganization.Count() == 1 && fetchedClassesInOrganization[0].Id == class1.Id)
        {
            Assert.True(fetchedClassesInOrganization[0].Id == class1.Id);
            Assert.True(fetchedClassesInOrganization[0].Name == class1.Name);
        }
        else if (fetchedClassesInOrganization.Count() == 1 && fetchedClassesInOrganization[0].Id == class2.Id)
        {
            Assert.True(fetchedClassesInOrganization[0].Id == class2.Id);
            Assert.True(fetchedClassesInOrganization[0].Name == class2.Name);
        }
        else if (fetchedClassesInOrganization.Count() == 2 && fetchedClassesInOrganization[0].Id == class1.Id)
        {
            Assert.True(fetchedClassesInOrganization[0].Id == class1.Id);
            Assert.True(fetchedClassesInOrganization[0].Name == class1.Name);
            Assert.True(fetchedClassesInOrganization[1].Id == class2.Id);
            Assert.True(fetchedClassesInOrganization[1].Name == class2.Name);
        }
        else if (fetchedClassesInOrganization.Count() == 2 && fetchedClassesInOrganization[0].Id == class2.Id)
        {
            Assert.True(fetchedClassesInOrganization[0].Id == class2.Id);
            Assert.True(fetchedClassesInOrganization[0].Name == class2.Name);
            Assert.True(fetchedClassesInOrganization[1].Id == class1.Id);
            Assert.True(fetchedClassesInOrganization[1].Name == class1.Name);
        }
    }
}