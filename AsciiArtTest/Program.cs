using AsciiArt;

using var file = File.OpenRead(@"..\..\..\..\logo.png");
var art = AsciiImage.Get(file);
Console.WriteLine(art);