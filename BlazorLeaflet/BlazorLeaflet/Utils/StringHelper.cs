using System;
using System.Linq;

namespace BlazorLeaflet.Utils;

public static class StringHelper
{
    private static readonly Random Random = new();

    public static string GetRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        return new(Enumerable.Repeat(chars, length)
            .Select(s => s[Random.Next(s.Length)])
            .ToArray());
    }
}