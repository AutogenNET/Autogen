using Autogen.Enum;

using System.Runtime.CompilerServices;

var w1 = Week.Sunday;
Console.WriteLine(w1.ToStringFast());

var w2 = Week.Monday;
Console.WriteLine(w2.ToStringFast());

foreach (var item in EnumExtensions.GetTexts<Week>())
{
    Console.WriteLine(item);
}

[AutogenEnum]
public enum Week
{
    [Text("일요일")]
    Sunday,
    Monday,
}




public static class EnumExtensions
{
    private static readonly EnumItem<Week>[] _weekTexts = new EnumItem<Week>[] {
        new EnumItem<Week>(nameof(Week.Sunday), Week.Sunday, "일요일"),
        new EnumItem<Week>(nameof(Week.Monday), Week.Monday, nameof(Week.Monday)),
    };

    public static IEnumerable<EnumItem<T>> GetTexts<T>()
        where T : Enum
    {
        var tType = typeof(T);
        if (tType == typeof(Week))
        {
            return (_weekTexts as IEnumerable<EnumItem<T>>)!;
        }
        else
            return Enumerable.Empty<EnumItem<T>>();
    }

    public record struct EnumItem<T>(string Member, T Value, string Text)
        where T : Enum
    {
        public override string ToString()
        {
            return Text;
        }
    }
}
