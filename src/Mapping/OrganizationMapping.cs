
public static class OrganizationMapping
{
    public static OrganizationDTO ToDTO(this Organization organization)
    {
        return new OrganizationDTO(
            organization.Id
        );
    }
}