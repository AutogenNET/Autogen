using Xunit;
using Autogen.Enum;
using System;
using System.Drawing;

namespace Autogen.IntegrationTests;

[AutogenEnum]
[Flags]
public enum Color
{
    Red = 1,
    Blue = 2,
    Green = 4,
}

public class EnumExtensionsTest
{
[Theory]
    [InlineData(Color.Red)]
    [InlineData(Color.Green)]
    [InlineData(Color.Green | Color.Blue)]
    [InlineData((Color)15)]
    [InlineData((Color)0)]
    public void FastToStringIsSameAsToString(Color value)
    {
        var expected = value.ToString();
        var actual = value.ToStringFast();

        Assert.Equal(expected, actual);
    }
}