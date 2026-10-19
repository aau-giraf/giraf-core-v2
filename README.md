## Guide

Run
`docker compose up --build`

Then check the following endpoint:
`localhost:8080/health`

To add entries to tables, run:
`docker exec -it giraf-core-v2-db-1 psql -U postgres -d defaultdb`

Now you can run `INSERT INTO ...;`

E.g., `INSERT INTO "Users" ("Id", "FirstName", "LastName", "Email", "Username", "Password", "Role") VALUES (1, 'Peter', 'Bødstrup', 'pb123@gmail.com', 'Hestepeter', 'hp123', 1);`

### Linux

For above guide on Linux run the following commands before `docker compose up --build`, to ensure a valid build platform is available.

`sudo docker buildx build .`

Might require the following package a separate 'docker buildx' package depending on your distro:
`sudo pacman -S docker-buildx`

## Migrations & New Database Tables

### Once, if dotnet-ef isn't installed

`dotnet tool install --global dotnet-ef --version 10.0.12`

### After adding your model and DbSet

`dotnet ef migrations add MIGRATION_NAME`

### Build and run; startup applies the migration

`docker compose up --build`

### Run tests

`docker compose --profile test run --run --rm test`

### Optional: discard the local database volume to fix the existing EnsureCreated/migration mismatch

### WARNING: deletes database data

`docker compose down -v`
`docker compose up --build`

## Run Tests

Run: `dotnet test --configuration Release --verbosity normal`
The test seed will then be visible in the error message if a test fails.

## Contributing

Before creating a pull request, please run `dotnet format`. This ensures consistent formatting between developers and reduces conflicts when merging.
