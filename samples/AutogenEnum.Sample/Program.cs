using Autogen.Enum;


var w1 = Week.Sunday;
Console.WriteLine(w1.ToStringFast());

var w2 = Week.Monday;
Console.WriteLine(w2.ToStringFast());


[AutogenEnum]
public enum Week
{
    [Text("일요일")]
    Sunday,
    Monday,
}
