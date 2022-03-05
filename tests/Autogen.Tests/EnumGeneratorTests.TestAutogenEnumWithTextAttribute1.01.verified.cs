﻿//HintName: Autogen.Enum.EnumExtensions.g.cs

namespace Autogen.Enum
{
    public static partial class EnumExtensions
    {
        public static string ToStringFast(this Color value) => value switch
        {
            Color.Red => "빨강",
            Color.Blue => "파랑",
            _ => value.ToString(),
        };
    }
}