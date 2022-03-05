//HintName: Autogen.Enum.EnumExtensions.g.cs

namespace Autogen.Enum
{
    public static partial class EnumExtensions
    {
        public static string ToStringFast(this Color value) => value switch
        {
            Color.Red => nameof(Color.Red),
            Color.Blue => nameof(Color.Blue),
            _ => value.ToString(),
        };
    }
}