using System.Threading.Tasks;

using VerifyXunit;

using Xunit;

namespace Autogen.Tests;

[UsesVerify]
public class EnumGeneratorTests
{
    [Fact]
    public Task GeneratesEnumExtensionsCorrectly()
    {
        var source = """
            using Autogen.Enum;

            [AutogenEnum]
            public enum Color
            {
                Red = 0,
                Blue = 1,
            }
            """;

        return TestHelper.Verify(source);
    }
}
