using System.Text.RegularExpressions;

namespace ExpressionEvaluator.Api.Security;

/// <summary>
/// Decides whether a browser Origin may call the API. Exact origins match as-is;
/// patterns allow a single host label to vary (e.g. "https://dashboard-*-team.vercel.app"
/// for Vercel preview URLs). A "*" never matches a dot, so a pattern cannot be
/// widened to an unrelated domain.
/// </summary>
public class OriginMatcher
{
    private readonly HashSet<string> _exact;
    private readonly List<Regex> _patterns;

    public OriginMatcher(IEnumerable<string> exactOrigins, IEnumerable<string> originPatterns)
    {
        _exact = new HashSet<string>(exactOrigins.Select(Normalize), StringComparer.OrdinalIgnoreCase);
        _patterns = originPatterns
            .Select(Normalize)
            .Where(p => p.Contains('*') && p != "*")
            .Select(p => new Regex(
                "^" + Regex.Escape(p).Replace("\\*", "[a-z0-9-]+") + "$",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
            .ToList();
    }

    public bool IsAllowed(string origin)
    {
        if (string.IsNullOrWhiteSpace(origin)) return false;
        var normalized = Normalize(origin);
        return _exact.Contains(normalized) || _patterns.Any(p => p.IsMatch(normalized));
    }

    private static string Normalize(string origin) => origin.Trim().TrimEnd('/');
}
