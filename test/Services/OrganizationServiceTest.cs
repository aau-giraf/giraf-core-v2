using Bogus.DataSets;
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
        Assert.Equal(organization2.Id, fetchedOrganization.Id);
        Assert.NotEqual(organization1.Id, fetchedOrganization.Id);
    }

    [Fact]
    public async Task GetOrganizationsList_GetsCorrectOrganizationsList()
    {   
        //----- ARRANGE -----\\
        await using var db = CreateDbContext();

        var organizations = await OrganizationSeeder.SeedManyAsync(db, Seed, 2);
        var organization1 = organizations[0];
        var organization2 = organizations[1];
        List<Organization> organizationsList = new List<Organization>(){organization1,organization2};

        Assert.NotNull(organization1);
        Assert.NotNull(organization2);

        var service = new OrganizationService(db);

        //----- ACT -----\\
        var fetchedOrganizations = await service.GetOrganizationsAsync();


        //----- ASSERT -----\\
        // UGLY: need to manualy go through all attributes of expected organization in List and check with fetched, because types. (solution: Mappers)
        Assert.True(organizationsList[0].Id == fetchedOrganizations[0].Id || organizationsList[0].Id == fetchedOrganizations[1].Id);
        Assert.True(organizationsList[1].Id == fetchedOrganizations[0].Id || organizationsList[1].Id == fetchedOrganizations[1].Id);

        /* ERROR: Fails without mappers
         * Without mappers: Argument 2: cannot convert from 'System.Collections.Generic.List<ResponseGetOrganizationDTO>'
         * to 'System.Collections.Generic.IAsyncEnumerable<giraf_core_v2.Models.Organization>?'
         * Assert.Equal(organizationsList, fetchedOrganizations);
        */
    }

    [Fact]
    public async Task DeleteOrganization_DeletesCorrectOrganization()
    {   
        //----- ARRANGE -----\\
        await using var db = CreateDbContext();

        var organizations = await UserSeeder.SeedManyAsync(db, Seed, 2);
        var organization1 = organizations[0];
        var organization2 = organizations[1];

        Assert.NotNull(organization1);
        Assert.NotNull(organization2);

        var service = new OrganizationService(db);

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
    public async Task DeleteClassInOrganization_DeletesCorrectClass()
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

        // Fetch the selected class in organization to be removed from that organisation
        var selectedClass = await db.Classes
            .Where(c => c.OrganizationId == organization1.Id).FirstOrDefaultAsync(c => c.Id == class1.Id);

        // Fetch the other class
        var notSelectedClass = await db.Classes
            .Where(c => c.OrganizationId == organization1.Id).FirstOrDefaultAsync(c => c.Id == class2.Id);
        

        //----- ACT -----\\
        // Delete the class1 from organization1
        var deletedClassInOrganization = await service.DeleteClassInOrganizationAsync(organization1.Id, class1.Id);


        //----- ASSERT -----\\
        if (selectedClass is not null)
        { 
            Assert.True(deletedClassInOrganization);
        } else if (selectedClass is null)
            {
                Assert.False(deletedClassInOrganization);
            }

        // After delition of class1 ensure that the state of other classes are not affected
        if (notSelectedClass is not null) 
        {   
            Assert.NotNull(await db.Classes
                .Where(c => c.OrganizationId == organization1.Id).FirstOrDefaultAsync(c => c.Id == class2.Id));
        } 
    }

}