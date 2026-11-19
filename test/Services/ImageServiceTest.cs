public class ImageServiceTest(ITestOutputHelper output) : TestBase(output)
{
    private readonly ImageSeeder _imageSeeder = new();
    private readonly UserSeeder _userSeeder = new();
    private readonly OrganizationSeeder _orgSeeder = new();
    private readonly ClassSeeder _classSeeder = new();

    [Fact]
    public async Task GetImagesByCitizenIdAsync_returnsTheListOfImages()
    {
        await using var db = CreateDbContext();
        var citizen = await _userSeeder.SeedAsync(db, Seed);
        List<ImageOptions> opt = [
            new(ClassId: null, CitizenId: citizen.User.Id),
            new(ClassId: null, CitizenId: citizen.User.Id),
            new(ClassId: null, CitizenId: citizen.User.Id)
        ];
        var images = await _imageSeeder.SeedManyAsync(db, seed: Seed, count: 3, opt);
        var service = new ImageService(db);

        var result = await service.GetImagesByCitizenIdAsync(citizen.User.Id);

        Assert.NotNull(result);
        Assert.Equal(3, result!.Count);
        Assert.Equal(
            images.Select(x => x.Id).OrderBy(id => id),
            result.Select(i => i.ImageId).OrderBy(id => id)
        );
    }

    [Fact]
    public async Task GetImagesByCitizenIdAsync_UserNotFound_ReturnsNull()
    {
        await using var db = CreateDbContext();
        var service = new ImageService(db);

        var result = await service.GetImagesByCitizenIdAsync(42);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetImagesByCitizenIdAsync_ReturnsOnlyImagesRelatedToCitizen()
    {
        await using var db = CreateDbContext();
        var citizens = await _userSeeder.SeedManyAsync(db, Seed, 2);
        var citizen1 = citizens[0];
        var citizen2 = citizens[1];

        List<ImageOptions> opt1 = [
            new(ClassId: null, CitizenId: citizen1.User.Id),
            new(ClassId: null, CitizenId: citizen1.User.Id),
            new(ClassId: null, CitizenId: citizen1.User.Id)
        ];

        List<ImageOptions> opt2 = [
            new(ClassId: null, CitizenId: citizen2.User.Id),
            new(ClassId: null, CitizenId: citizen2.User.Id),
            new(ClassId: null, CitizenId: citizen2.User.Id)
        ];

        var seededImages = await _imageSeeder.SeedManyAsync(db, Seed, count: 3, opt1);
        var irrelevantImages = await _imageSeeder.SeedManyAsync(db, Seed, count: 3, opt2);
        var service = new ImageService(db);

        var result = await service.GetImagesByCitizenIdAsync(citizen1.User.Id);

        Assert.NotNull(result);
        Assert.Equal(3, result!.Count);
        Assert.Equal(
            seededImages.Select(x => x.Id).OrderBy(id => id),
            result.Select(i => i.ImageId).OrderBy(id => id)
        );
    }

    [Fact]
    public async Task GetImagesByCitizenIdAsync_UserFound_HasNoImages()
    {
        await using var db = CreateDbContext();
        var citizen = await _userSeeder.SeedAsync(db, seed: Seed);
        var service = new ImageService(db);

        var result = await service.GetImagesByCitizenIdAsync(citizen.User.Id);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetImagesByClassIdAsync_ClassFound_HasNoImages()
    {
        await using var db = CreateDbContext();
        var class_ = await _classSeeder.SeedAsync(db, seed: Seed);
        var service = new ImageService(db);

        var result = await service.GetImagesByClassIdAsync(class_.Id);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetImagesByClassIdAsync_returnsTheListOfImages()
    {
        await using var db = CreateDbContext();
        var class_ = await _classSeeder.SeedAsync(db, Seed);
        List<ImageOptions> opt = [
            new(ClassId: class_.Id, CitizenId: null),
            new(ClassId: class_.Id, CitizenId: null),
            new(ClassId: class_.Id, CitizenId: null)
        ];
        var images = await _imageSeeder.SeedManyAsync(db, seed: Seed, count: 3, opt);
        var service = new ImageService(db);

        var result = await service.GetImagesByClassIdAsync(class_.Id);

        Assert.NotNull(result);
        Assert.Equal(3, result!.Count);
        Assert.Equal(
            images.Select(x => x.Id).OrderBy(id => id),
            result.Select(i => i.ImageId).OrderBy(id => id)
        );
    }

    [Fact]
    public async Task GetImagesByClassIdAsync_ClassNotFound_ReturnsNull()
    {
        await using var db = CreateDbContext();
        var service = new ImageService(db);

        var result = await service.GetImagesByClassIdAsync(42);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetImagesByClassIdAsync_ReturnsOnlyImagesRelatedToClass()
    {
        await using var db = CreateDbContext();
        var classes = await _classSeeder.SeedManyAsync(db, Seed, 2);
        var class1 = classes[0];
        var class2 = classes[1];

        List<ImageOptions> opt1 = [
            new(ClassId: class1.Id, CitizenId: null),
            new(ClassId: class1.Id, CitizenId: null),
            new(ClassId: class1.Id, CitizenId: null)
        ];

        List<ImageOptions> opt2 = [
            new(ClassId: class2.Id, CitizenId: null),
            new(ClassId: class2.Id, CitizenId: null),
            new(ClassId: class2.Id, CitizenId: null)
        ];

        var seededImages = await _imageSeeder.SeedManyAsync(db, Seed, count: 3, opt1);
        var irrelevantImages = await _imageSeeder.SeedManyAsync(db, Seed, count: 3, opt2);
        var service = new ImageService(db);

        var result = await service.GetImagesByClassIdAsync(class1.Id);

        Assert.NotNull(result);
        Assert.Equal(3, result!.Count);
        Assert.Equal(
            seededImages.Select(x => x.Id).OrderBy(id => id),
            result.Select(i => i.ImageId).OrderBy(id => id)
        );
    }

    [Fact]
    public async Task GetImagesByOrganizationIdAsync_OrganizationFound_HasNoImages()
    {
        await using var db = CreateDbContext();
        var organization = await _orgSeeder.SeedAsync(db, seed: Seed);
        var service = new ImageService(db);

        var result = await service.GetImagesByClassIdAsync(organization.Id);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetImagesByOrganizationIdAsync_returnsTheListOfImages()
    {
        await using var db = CreateDbContext();
        var organization = await _orgSeeder.SeedAsync(db, Seed);
        List<ImageOptions> opt = [
            new(ClassId: null, CitizenId: null, OrgId: organization.Id),
            new(ClassId: null, CitizenId: null, OrgId: organization.Id),
            new(ClassId: null, CitizenId: null, OrgId: organization.Id)
        ];
        var images = await _imageSeeder.SeedManyAsync(db, seed: Seed, count: 3, opt);
        var service = new ImageService(db);

        var result = await service.GetImagesByClassIdAsync(organization.Id);

        Assert.NotNull(result);
        Assert.Equal(3, result!.Count);
        Assert.Equal(
            images.Select(x => x.Id).OrderBy(id => id),
            result.Select(i => i.ImageId).OrderBy(id => id)
        );
    }

    [Fact]
    public async Task GetImagesByOrganizationIdAsync_OrganizationNotFound_ReturnsNull()
    {
        await using var db = CreateDbContext();
        var service = new ImageService(db);

        var result = await service.GetImagesByClassIdAsync(42);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetImagesByOrganizationIdAsync_ReturnsOnlyImagesRelatedToOrganization()
    {
        await using var db = CreateDbContext();
        var organizations = await _orgSeeder.SeedManyAsync(db, Seed, 2);
        var organization1 = organizations[0];
        var organization2 = organizations[1];

        List<ImageOptions> opt1 = [
            new(ClassId: null, CitizenId: null, OrgId: organization1.Id),
            new(ClassId: null, CitizenId: null, OrgId: organization1.Id),
            new(ClassId: null, CitizenId: null, OrgId: organization1.Id)
        ];

        List<ImageOptions> opt2 = [
            new(ClassId: null, CitizenId: null, OrgId: organization2.Id),
            new(ClassId: null, CitizenId: null, OrgId: organization2.Id),
            new(ClassId: null, CitizenId: null, OrgId: organization2.Id)
        ];

        var seededImages = await _imageSeeder.SeedManyAsync(db, Seed, count: 3, opt1);
        var irrelevantImages = await _imageSeeder.SeedManyAsync(db, Seed, count: 3, opt2);
        var service = new ImageService(db);

        var result = await service.GetImagesByClassIdAsync(organization1.Id);

        Assert.NotNull(result);
        Assert.Equal(3, result!.Count);
        Assert.Equal(
            seededImages.Select(x => x.Id).OrderBy(id => id),
            result.Select(i => i.ImageId).OrderBy(id => id)
        );
    }


    [Fact]
    public async Task DeleteImageAsync_DeleteImage()
    {
        await using var db = CreateDbContext();

        var image = await _imageSeeder.SeedAsync(db, Seed);
        var dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(dir);
        try
        {
            image.Path = Path.Combine(dir, Path.GetFileName(image.Path));
            await db.SaveChangesAsync();
            File.WriteAllBytes(image.Path, []);
            var service = new ImageService(db);

            Assert.True(await service.DeleteImageAsync(image.Id));
            Assert.Null(await db.Images.SingleOrDefaultAsync(i => i.Id == image.Id));
            Assert.False(File.Exists(image.Path));
        }
        finally
        {
            Directory.Delete(dir, recursive: true);
        }
    }

    [Fact]
    public async Task DeleteImageAsync_DeletesRightImage()
    {
        await using var db = CreateDbContext();

        var images = await _imageSeeder.SeedManyAsync(db, Seed, 2);
        var image1 = images[0];
        var image2 = images[1];

        var dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(dir);

        try
        {
            image1.Path = Path.Combine(dir, Path.GetFileName(image1.Path));  // Makes the paths more controlled (Only the filename from seeder in our own folder)
            image2.Path = Path.Combine(dir, Path.GetFileName(image2.Path));
            await db.SaveChangesAsync();

            File.WriteAllBytes(image1.Path, []); // // opens and close the file
            File.WriteAllBytes(image2.Path, []);

            var service = new ImageService(db);

            Assert.True(await service.DeleteImageAsync(image1.Id));

            Assert.Null(await db.Images.SingleOrDefaultAsync(i => i.Id == image1.Id));
            Assert.False(File.Exists(image1.Path));

            Assert.NotNull(await db.Images.SingleOrDefaultAsync(i => i.Id == image2.Id));
            Assert.True(File.Exists(image2.Path));
        }
        finally
        {
            Directory.Delete(dir, recursive: true);
        }
    }

    [Fact]
    public async Task DeleteImageAsync_ReturnsFalseIfIdDontExists()
    {
        await using var db = CreateDbContext();
        var service = new ImageService(db);
        var nonExistingImageId = 9999;

        var result = await service.DeleteImageAsync(nonExistingImageId);

        Assert.False(result);
    }


    [Fact]
    public async Task PostImageAsync_PostImage()
    {
        await using var db = CreateDbContext();
        var organization = await _orgSeeder.SeedAsync(db, Seed);
        var request = _imageSeeder.CreateRequestBody(Seed, new(OrgId: organization.Id, ClassId: null, CitizenId: null));
        var service = new ImageService(db);
        var created = await service.CreateImagesAsync(request);
        try
        {
            Assert.NotNull(created);
            Assert.Equal(request.Name, created!.Title);
            Assert.Equal(request.OrganizationId, created.OrganizationId);
            Assert.Equal(request.CitizenId, created.CitizenId);
            Assert.Equal(request.ClassId, created.ClassId);
            Assert.Equal(Path.GetExtension(request.File.FileName), Path.GetExtension(created.Path));
            Assert.True(File.Exists(created.Path));
            var stored = await db.Images.SingleOrDefaultAsync(i => i.Id == created.ImageId);
            Assert.NotNull(stored);
            Assert.Equal(created.Path, stored!.Path);
        }
        finally
        {
            if (created?.Path is not null && File.Exists(created.Path))
            {
                File.Delete(created.Path);
            }
        }
    }

    [Fact]
    public async Task PostImageAsync_PostsTheRightImage()
    {
        await using var db = CreateDbContext();
        var organization = await _orgSeeder.SeedAsync(db, Seed);
        var request = _imageSeeder.CreateRequestBody(Seed, new(OrgId: organization.Id, ClassId: null, CitizenId: null));
        var service = new ImageService(db);
        var created = await service.CreateImagesAsync(request);
        try
        {
            Assert.NotNull(created);
            var allImages = await db.Images.ToListAsync();
            var stored = Assert.Single(allImages);
            Assert.Equal(created!.ImageId, stored.Id);
            Assert.Equal(request.Name, stored.Name);
            Assert.Equal(request.OrganizationId, stored.OrganizationId);
            Assert.Equal(request.CitizenId, stored.CitizenId);
            Assert.Equal(request.ClassId, stored.ClassId);

            Assert.Equal(created.Path, stored.Path);
            Assert.True(File.Exists(stored.Path));
        }
        finally
        {
            if (created?.Path is not null && File.Exists(created.Path))
            {
                File.Delete(created.Path);
            }
        }
    }


    [Fact]
    public async Task FindOrganizationAsync_requestKnowingCitizenId()
    {
        await using var db = CreateDbContext();
        var organization = await _orgSeeder.SeedAsync(db, Seed);
        var class_ = await _classSeeder.SeedAsync(db, Seed, new(OrgId: organization.Id));
        var citizen = await _userSeeder.SeedAsync(db, Seed, new(ClassId: class_.Id));
        var request = _imageSeeder.CreateRequestBody(Seed, new(OrgId: null, ClassId: null, CitizenId: citizen.User.Id));

        var service = new ImageService(db);
        var created = await service.CreateImagesAsync(request);
        try
        {
            Assert.NotNull(created);
            Assert.Equal(request.Name, created!.Title);
            Assert.Equal(citizen.User.Id, created.CitizenId);
            Assert.Null(created.ClassId);
            Assert.Equal(class_.Id, created.OrganizationId);
            Assert.Equal(Path.GetExtension(request.File.FileName), Path.GetExtension(created.Path));
            Assert.True(File.Exists(created.Path));

            var stored = await db.Images.SingleOrDefaultAsync(i => i.Id == created.ImageId);
            Assert.NotNull(stored);
            Assert.Equal(created.Path, stored!.Path);
            Assert.Null(stored.ClassId);
            Assert.Equal(citizen.User.Id, stored.CitizenId);
            Assert.Equal(class_.OrganizationId, stored.OrganizationId);
        }
        finally
        {
            if (created?.Path is not null && File.Exists(created.Path))
            {
                File.Delete(created.Path);
            }
        }
    }

    [Fact]
    public async Task PostImageAsync_requestKnowingClassId()
    {
        await using var db = CreateDbContext();
        var organization = await _orgSeeder.SeedAsync(db, Seed);
        var class_ = await _classSeeder.SeedAsync(db, Seed, new(OrgId: organization.Id));
        var request = _imageSeeder.CreateRequestBody(Seed, new(OrgId: null, CitizenId: null, ClassId: class_.Id));

        var service = new ImageService(db);
        var created = await service.CreateImagesAsync(request);
        try
        {
            Assert.NotNull(created);
            Assert.Equal(request.Name, created!.Title);
            Assert.Null(created.CitizenId);
            Assert.Equal(class_.Id, created.ClassId);
            Assert.Equal(organization.Id, created.OrganizationId);
            Assert.Equal(Path.GetExtension(request.File.FileName), Path.GetExtension(created.Path));
            Assert.True(File.Exists(created.Path));

            var stored = await db.Images.SingleOrDefaultAsync(i => i.Id == created.ImageId);
            Assert.NotNull(stored);
            Assert.Equal(created.Path, stored!.Path);
            Assert.Equal(class_.Id, stored.ClassId);
            Assert.Null(stored.CitizenId);
            Assert.Equal(organization.Id, stored.OrganizationId);
        }
        finally
        {
            if (created?.Path is not null && File.Exists(created.Path))
            {
                File.Delete(created.Path);
            }
        }
    }

    [Fact]
    public async Task MultipleServiceComponentsTest()
    {
        await using var db = CreateDbContext();
        var organization = await _orgSeeder.SeedAsync(db, Seed);
        var request = _imageSeeder.CreateRequestBody(
            Seed,
            new(OrgId: organization.Id, ClassId: null, CitizenId: null));

        var service = new ImageService(db);
        string? path = null;

        try
        {
            // POST
            var created = await service.CreateImagesAsync(request);

            Assert.NotNull(created);
            path = created.Path;
            Assert.True(File.Exists(path));

            db.ChangeTracker.Clear();

            // GET by OrganizationId
            var fetched = await service.GetImagesByOrganizationIdAsync(organization.Id);

            Assert.NotNull(fetched);
            Assert.Single(fetched);

            var fetchedImage = fetched[0];

            Assert.Equal(created.ImageId, fetchedImage.ImageId);
            Assert.Equal(created.Title, fetchedImage.Title);

            db.ChangeTracker.Clear();

            // DELETE
            Assert.True(await service.DeleteImageAsync(created.ImageId));

            db.ChangeTracker.Clear();

            // GET after DELETE
            var imagesAfterDelete = await service.GetImagesByOrganizationIdAsync(organization.Id);

            Assert.NotNull(imagesAfterDelete);
            Assert.Empty(imagesAfterDelete);
            Assert.Empty(await db.Images.ToListAsync());
            Assert.False(File.Exists(path));
        }
        finally
        {
            if (path is not null && File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}