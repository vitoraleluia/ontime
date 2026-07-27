using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace OnTime.Domain.Common;

public static class StringExtensions
{
    public static string RemoveDiacritics(this string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder(normalizedString.Length);

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    public static string BuildFriendlyUrl(this string text, int maxLength = 50)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        var clean = text.RemoveDiacritics().ToLowerInvariant();

        // Replace any character that is not lowercase letter, digit, space, or hyphen with empty string
        clean = Regex.Replace(clean, @"[^a-z0-9\s-]", "");

        // Convert spaces and consecutive hyphens into a single hyphen
        clean = Regex.Replace(clean, @"[\s-]+", "-").Trim('-');

        if (clean.Length > maxLength)
        {
            clean = clean.Substring(0, maxLength).TrimEnd('-');
        }

        return clean;
    }
}
