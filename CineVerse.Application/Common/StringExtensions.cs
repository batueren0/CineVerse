using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace CineVerse.Application.Common;

public static class StringExtensions
{
    public static string ToKebabCase(this string value)
    {
        var slug = value.Trim()
            .Replace("İ", "i").Replace("I", "i")
            .ToLowerInvariant()
            .Replace("ç", "c").Replace("ğ", "g").Replace("ı", "i")
            .Replace("ö", "o").Replace("ş", "s").Replace("ü", "u");

        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");  // harf, rakam, boşluk ve tire dışındakileri sil
        slug = Regex.Replace(slug, @"[\s-]+", "-");       // boşluk ve tire gruplarını tek tireye indir

        return slug.Trim('-');
    }

}
