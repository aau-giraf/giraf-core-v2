using System.Security.Cryptography.X509Certificates;
using Bogus.DataSets;
using Microsoft.OpenApi;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Xunit.Abstractions;
using Xunit.Sdk;

public class OrganizationServiceTest : TestBase
{
    public OrganizationServiceTest(ITestOutputHelper output) : base(output) { }

    // used to offset seed for different results.
    private readonly int SEED_OFFSET = 124;

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

        List<Organization> organizationsList = new List<Organization>(){organization1, organization2};

        Assert.NotNull(organization1);
        Assert.NotNull(organization2);

        var service = new OrganizationService(db);

        //----- ACT -----\\
        var fetchedOrganizations = await service.GetOrganizationsAsync();

        //----- ASSERT -----\\
        // Ugly: Properly a better way (We also dont know if we get list in same order, which this requires)
        Assert.True(organizationsList[0].Id == fetchedOrganizations[0].Id || organizationsList[0].Id == fetchedOrganizations[1].Id);
        Assert.True(organizationsList[1].Id == fetchedOrganizations[0].Id || organizationsList[1].Id == fetchedOrganizations[1].Id);
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
        } else if (selectedClass is null && class1.OrganizationId != organization1.Id)
        {   
            // If the class is not in the organization it must be in the other organization
            Assert.True(selectedClass.OrganizationId == organization2.Id);
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
            Assert.True(class2.Name == fetchedClass.Name);
            Assert.True(class2.OrganizationId == fetchedClass.OrganizationId);
        } else
        {   
            // Assert that if it isnt in organization1 then it must be in organization2
            Assert.True(organization2.Id == fetchedClass.OrganizationId);
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
            Name = "MunkholmSkolen",
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

        Assert.NotNull(organization1);
        Assert.NotNull(organization2);

        var service = new OrganizationService(db);

        // Create the dummy DTO for the requestbody
        RequestCreateOrganizationDTO requestCreateOrganizationDTO = new RequestCreateOrganizationDTO
        {
            Name = organization1.Name,
        };

        //----- ACT -----\\
        var updatedOrganization = await service.UpdateOrganizationAsync(requestCreateOrganizationDTO, organization1.Id);

        //----- ASSERT -----\\
        Assert.NotNull(updatedOrganization);
        Assert.True(requestCreateOrganizationDTO.Name == updatedOrganization.Name);
        Assert.True(organization1.Id == updatedOrganization.Id);
        // Asserts that the new name for the organization is actually new
        Assert.True(organization1.Name != updatedOrganization.Name);
        // Asserts that you cant rename and organization to an existing one
        Assert.True(organization2.Name != updatedOrganization.Name);
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

        // Asserts that the created class does contain a name of a class already in the organization.
        if (class2.OrganizationId == organization1.Id)
        {
            Assert.False(createdClassInOrganization.Name == class2.Name);
        }
    }

}

