using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace CineVerse.Application.Common;

public static class StringExtensions
{
    public static string ToKebabCase(this string value)
    {
        return value
            .Trim()
            .ToLowerInvariant()
            .Replace(" ","-");
    }

}
