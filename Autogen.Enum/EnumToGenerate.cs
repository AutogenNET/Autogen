namespace Autogen.Enum;

public struct EnumToGenerate
{
    public readonly string Name;
    public readonly IEnumerable<string> Values;

    public EnumToGenerate(string name, IEnumerable<string> values)
    {
        Name = name;
        Values = values;
    }
}
