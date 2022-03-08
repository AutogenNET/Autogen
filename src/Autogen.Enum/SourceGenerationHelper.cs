using System.Text;

namespace Autogen.Enum;

internal static class SourceGenerationHelper
{
    public const string Attribute =
@"
namespace Autogen.Enum
{
    [System.AttributeUsage(System.AttributeTargets.Enum)]
    public class AutogenEnumAttribute : System.Attribute
    {
    }

    [System.AttributeUsage(System.AttributeTargets.All)]
    public class TextAttribute : System.Attribute
    {
        public string Text { get; }

        public TextAttribute(string text)
        {
            Text = text;
        }
    }
}
";

    public static string GenerateExtensionClass(IEnumerable<EnumToGenerate> enumsToGenerate)
    {
        var sb = new StringBuilder();
        sb.Append(@"
namespace Autogen.Enum
{
    public static partial class EnumExtensions
    {");
        foreach (var enumToGenerate in enumsToGenerate)
        {
            sb.Append(@"
        public static string ToStringFast(this ").Append(enumToGenerate.Name).Append(@" value) => value switch
        {");
            foreach (var (Member, Text) in enumToGenerate.Values)
            {
                sb.Append(@"
            ").Append(enumToGenerate.Name).Append('.').Append(Member).Append(" => ")
                .Append(Text).Append(",");
            }

            sb.Append(@"
            _ => value.ToString(),
        };");
        }

        sb.Append(@"
    }
}");

        return sb.ToString();
    }
}
