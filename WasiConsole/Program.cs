using System;
using System.IO;
using System.Threading;
using AsciiArt;

namespace WasiConsole;

public class Program
{
    public static void Hello(string text)
    {
        Native.Print($"Hello {text}");
    }

    public static int Fibonacci(int len)
    {
        int a = 0, b = 1, c = 0;
        for (int i = 2; i < len; i++)
        {
            c= a + b;
            a= b;
            b= c;
        }

        return c;
    }

    static void Main()
    {
        Console.WriteLine("Started!");
        File.WriteAllText("date.txt", DateTime.Now.ToString("O"));

        var loop = Convert.ToBoolean(Environment.GetEnvironmentVariable("loop") ?? "false");
        if (loop)
        {
            using var ms = new MemoryStream(Logo.Data);
            Console.WriteLine(AsciiImage.Get(ms));
        }

        while (loop)
        {
            Console.WriteLine("Ping");
            Thread.Sleep(3000);
        }
    }

}