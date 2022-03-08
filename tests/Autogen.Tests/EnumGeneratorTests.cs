using System.Threading.Tasks;

using VerifyXunit;

using Xunit;

namespace Autogen.Tests;

[UsesVerify]
public class EnumGeneratorTests
{
    [Fact]
    public Task TestAutogenEnum()
    {
        var source =
@"
            using Autogen.Enum;

            [AutogenEnum]
            public enum Color
            {
                Red,
                Blue,
            }
";

        return TestHelper.Verify(source);
    }

    [Fact]
    public Task TestAutogenEnumWithTextAttribute1()
    {
        var source =
@"
            using Autogen.Enum;

            [AutogenEnum]
            public enum Color
            {
                [Text(""»¡°­"")]
                Red = 0,
                [Text(""ÆÄ¶û"")]
                Blue = 1,
            }
";

        return TestHelper.Verify(source);
    }

    [Fact]
    public Task TestAutogenEnumWithTextAttribute2()
    {
        var source =
@"
            using Autogen.Enum;

            [AutogenEnum]
            public enum Color
            {
                [Text(""»¡°­"")]
                Red = 0,
                Blue = 1,
                [Text(""³ë¶û"")]    
                Yellow = 2,
            }
  ";

        return TestHelper.Verify(source);
    }
}
