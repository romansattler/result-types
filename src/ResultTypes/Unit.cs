namespace ResultTypes;

public record class Unit
{
    public static Unit Instance { get; } = new Unit();

    private Unit()
    {
    }
}