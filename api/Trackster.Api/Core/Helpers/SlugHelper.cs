using System.Text.RegularExpressions;

namespace Trackster.Api.Core.Helpers;

public class SlugHelper
{
    public static string GenerateSlugFor(string? title)
    {
        if (string.IsNullOrEmpty(title))
            return Guid.NewGuid().ToString();
        
        var slug = title.ToLower();
        
        slug = slug
            .Replace("&", "and")
            .Replace(" - ", " ")
            .Replace(".", "")
            .Replace("ą", "a")
            .Replace("ć", "c")
            .Replace("ę", "e")
            .Replace("ł", "l")
            .Replace("ń", "n")
            .Replace("ó", "o")
            .Replace("ś", "s")
            .Replace("ż", "z")
            .Replace("ź", "z");
        
        slug = Regex.Replace(slug, "[^a-zA-Z0-9_. ]+", "", RegexOptions.Compiled);
        
        slug = slug.Replace(' ', '-');

        return slug;
    }
}