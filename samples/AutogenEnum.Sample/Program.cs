using Autogen.Enum;


var w = Week.Sunday;
Console.WriteLine(w.ToStringFast());


[AutogenEnum]
public enum Week
{
    [Text("일요일")]
    Sunday,
}
