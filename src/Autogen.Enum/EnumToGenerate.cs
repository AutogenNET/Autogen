namespace Autogen.Enum;

public struct EnumToGenerate
{
    public readonly string Name;
    public readonly IEnumerable<(string Member, string Text)> Values;

    public EnumToGenerate(string name, IEnumerable<(string Member, string Text)> values)
    {
        Name = name;
        Values = values;
    }
}
