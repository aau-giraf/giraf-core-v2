
public static class ClassMapping
{
    public static ClassDTO ToDTO(this Class classEntity)
    {
        return new ClassDTO(
            classEntity.Id
        );
    }
}

