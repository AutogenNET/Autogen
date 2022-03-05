//HintName: Autogen.SourceGeneration.SourceGenerator.g.cs
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