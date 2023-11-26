using System.Text;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace AsciiArt;

public static class AsciiImage
{

    public static string Get(Stream stream)
    {
        using var image = Image.Load<Rgba32>(stream);

        return ConvertToAsciiArt(image);
    }

    private static string ConvertToAsciiArt(Image<Rgba32> image)
    {
        var asciiChars = "@%#*+=-:,. ";

        var aspect = image.Width / (double)image.Height;
        var outputWidth = 40;
        var widthStep = image.Width / outputWidth;
        var outputHeight = (int)(outputWidth / aspect);
        var heightStep = image.Height / outputHeight;

        StringBuilder asciiBuilder = new(outputWidth * outputHeight);
        for (var h = 0; h < image.Height; h += heightStep)
        {
            for (var w = 0; w < image.Width; w += widthStep)
            {
                var pixelColor = image[w, h];
                var grayValue = (int)(pixelColor.R * 0.3 +
                                      pixelColor.G * 0.59 + pixelColor.B * 0.11);
                var asciiChar = asciiChars[grayValue * (asciiChars.Length - 1) / 255];
                asciiBuilder.Append(asciiChar);
                asciiBuilder.Append(asciiChar);
            }

            asciiBuilder.AppendLine();
        }

        return asciiBuilder.ToString();
    }

}
