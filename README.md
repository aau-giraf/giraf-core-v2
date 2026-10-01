## Guide
Run
`docker compose up --build`

Then check the following endpoints:
`localhost:8080/health`
`localhost:8080/colors`

The `/colors` endpoint should return an empty array. To add entries to the colors table, run:
`docker exec -it giraf-core-v2-db-1 psql -U postgres -d defaultdb`

Now you can run `INSERT INTO "Colors" ("Id", "ColorName") VALUES (1, 'Red');`

### Linux
For above guide on Linux run the following commands before `docker compose up --build`, to ensure a valid build platform is available.

`sudo docker buildx build .`

Might require the following package a separate 'docker buildx' package depending on your distro:
`sudo pacman -S docker-buildx`

## Migrations & New Database Tables
### Once, if dotnet-ef isn't installed
dotnet tool install --global dotnet-ef --version 10.0.12

### After adding your model and DbSet
dotnet ef migrations add AddProducts --project src/giraf-core-v2.csproj --startup-project src/giraf-core-v2.csproj

### Build and run; startup applies the migration
docker compose up --build

### Optional: discard the local database volume to fix the existing EnsureCreated/migration mismatch
### WARNING: deletes database data
docker compose down -v
docker compose up --build