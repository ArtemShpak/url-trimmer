using UrlShortener.Application.Interfaces;
using UrlShortener.Application.Interfaces.Services;

namespace UrlShortener.Application.Services;

public class UrlShortenerService : IUrlShortenerService
{
    private const string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private readonly Random _random = new();

    public string GenerateShortCode()
    {
        var chars = new char[6];
        for (int i = 0; i < 6; i++)
        {
            chars[i] = Alphabet[_random.Next(Alphabet.Length)];
        }

        return new string(chars);
    }
}
