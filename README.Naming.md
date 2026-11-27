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
  - Exception: A model class, its corresponding `IEntityTypeConfiguration<Model>` implementation (EF Configuration), and enums specific to that model may share the model's file. This decision aims to improve cohesion of model-database mapping, whether configured through data annotations or Fluent API.
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

- Properties are PascalCase.
- Single-property (simple) primary keys are named: `Id`.
  - Potential *natural* keys may be considered an exception, but must be carefully evaluated before use - regarding stability, size/performance & security/privacy.

- *Foreign-key* property names follow `<navigation property name>Id` (`GuardianId` for `Guardianship.Guardian`).
  - If the referenced model's primary key falls under the above *natural*-key naming exception, or an alternate key is used as reference, use the generalized rule `<navigation property name><key property name>`.
- *Reference navigation* property names should communicate the domain relationship.
  - Use the related type name by default (`Citizen.Class`).
  - If a domain-specific name makes the relationship more explicit, use it (`Guardianship.Guardian` refers to a `NonCitizen` in the guardian relationship).
- *Collection navigation* property names are plural.
  - Use the plural related type name by default (`Organization.Images`).
  - For a clearer domain expression, name the collection from the declaring entity's perspective, especially for bridge entities (`Class.EmployeeRoles` and `EmployeeRole.ClassRoles` are collections of `ClassEmployeeRole`).
- While these conventions guide names, not model configuration, names can affect EF Core relationship discovery by convention, so consult Microsoft’s [Create a model](https://learn.microsoft.com/en-us/ef/core/modeling/) and [Conventions for relationship discovery](https://learn.microsoft.com/en-us/ef/core/modeling/relationships/conventions) documentation when configuring the model.

## Routes

- Route segments are plural camelCase. Example: `/citizens`, `/nonCitizens`, `/employeeRoles`.
- Route parameters are camelCase, matching method parameters. Example: `{nonCitizenId}`.

## Abbreviations

- Spell names out. Example: `ClassId`, `OrganizationId`.
- Abbreviations are allowed in lambda function. 
- Avoid short forms such as `orgId` and `cit_id`.

## Tests

- Name a test method `Method_Condition_Result`. Example: `GetCitizenById_CitizenExists_ReturnsCorrectCitizen`.
