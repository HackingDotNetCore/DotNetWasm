using System;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices.JavaScript;
using AsciiArt;

Console.WriteLine("Hello, Browser!");

public partial class MyClass
{
    [JSExport]
    internal static string Greeting()
    {
        var text = $"Hello, World! Greetings from {GetHRef()}. It's {DateTime.Now}";
        Console.WriteLine(text);

        return text;
    }

    [JSImport("window.location.href", "main.js")]
    internal static partial string GetHRef();

    [JSExport]
    internal static string GetAsciiArt(byte[] image)
    {
        using var ms = new MemoryStream(image);
        return AsciiImage.Get(ms);
    }
}
