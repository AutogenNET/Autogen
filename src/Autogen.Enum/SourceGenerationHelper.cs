using System.Text;

namespace Autogen.Enum;

internal static class SourceGenerationHelper
{
    public const string Attribute = """
        namespace Autogen.Enum
        {
            [System.AttributeUsage(System.AttributeTargets.Enum)]
            public class AutogenEnumAttribute : System.Attribute
            {
            }
        }
        """;

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
            foreach (var member in enumToGenerate.Values)
            {
                sb.Append(@"
            ").Append(enumToGenerate.Name).Append('.').Append(member)
                .Append(" => nameof(")
                .Append(enumToGenerate.Name).Append('.').Append(member).Append("),");
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
