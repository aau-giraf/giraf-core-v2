# Naming conventions

## Methods

- Method names are PascalCase. Example: `GetCitizenAsync`.
- Async methods end with `Async`. Example: `GetCitizenAsync`.

## Parameters and local variables

- Parameters and local variables are camelCase. Example: `nonCitizenId`, `classId`, `organizationId`.

## Fields

- `readonly` fields start with `_`, then camelCase. Example: `_db`.

## Types

- Classes, records, and enums are PascalCase.
- The file name matches the type name, with one public type per file.
- The namespace matches the folder under `giraf_core_v2`.
- Interfaces use an `I` prefix. Example: `ISeeding`.
- Enums use a singular PascalCase name, PascalCase members, and no `Enum` suffix. Example: `RoleType`.

## Layers
| Layer | Pattern | Example |
| --- | --- | --- |
| Service | `Services/<Name>Service` | `CitizenService` |
| Endpoints | `Endpoints/<Name>Endpoints`, registered with `Map<Name>Endpoints` | `CitizenEndpoints`, `MapCitizenEndpoints` |
| Seeding | `Data/Seeding/<Name>Seeding` implementing `ISeeding` | `CitizenSeeding` |
| Test seeder | `<Name>Seeder` | `CitizenSeeder` |
| Service test | `<Name>ServiceTest` | `CitizenServiceTest` |
| Endpoint test | `<Name>EndpointTest` | `CitizenEndpointTest` |
| EF configuration | `<Name>Configuration` | `UserConfiguration` |

## DTOs

- Name a DTO `<functionality><type>DTO`. Example: `GetCitizenResponseDTO`.
- Properties are PascalCase. Example: `ClassId`.
- IDs name the entity. Example: `UserId`, `ClassId`.

## Models

- Properties are PascalCase. Example: `ClassId`.
- The primary key is `Id`.
  - Unless the primary key is a composite key properties will be explicitly written. Example `CitizenID` and `GuardianId`
- A foreign key is the navigation name plus `Id`. Example: `ClassId`, `OrganizationId`.
- A navigation property uses the related type name. Example: `Class`, `Organization`.
- Collections are plural. Example: `Classes`, `Images`, `Guardianships`.

## Routes

- Route segments are plural camelCase. Example: `/citizens`, `/nonCitizens`, `/userRoles`.
- Route parameters are camelCase, matching method parameters. Example: `{nonCitizenId}`.

## Abbreviations

- Spell names out. Example: `ClassId`, `OrganizationId`.
- Abbreviations are allowed in lambda function. 
- Avoid short forms such as `orgId` and `cit_id`.

## Tests

- Name a test method `Method_Condition_Result`. Example: `GetCitizenById_CitizenExists_ReturnsCorrectCitizen`.
