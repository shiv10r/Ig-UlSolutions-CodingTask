using System.Text.RegularExpressions;

namespace ExpressionEvaluator.Api.Security;

/// <summary>
/// Decides whether a browser Origin may call the API. Exact origins match as-is;
/// a pattern may vary one part of one host label (e.g. "https://dashboard-*-team.vercel.app"
/// for Vercel preview URLs). Patterns that could match unrelated domains are rejected at
/// startup, so a config typo can't silently open CORS.
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
            .Where(p => p.Length > 0)
            .Select(p =>
            {
                Validate(p);
                return new Regex(
                    "^" + Regex.Escape(p).Replace("\\*", "[a-z0-9-]+") + "$",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            })
            .ToList();
    }

    public bool IsAllowed(string origin)
    {
        if (string.IsNullOrWhiteSpace(origin)) return false;
        var normalized = Normalize(origin);
        return _exact.Contains(normalized) || _patterns.Any(p => p.IsMatch(normalized));
    }

    // Safe shape: scheme://<label-with-fixed-text-and-one-*>.<fixed>.<fixed>[...][:port]
    private static void Validate(string pattern)
    {
        var schemeEnd = pattern.IndexOf("://", StringComparison.Ordinal);
        var host = schemeEnd < 0 ? "" : pattern[(schemeEnd + 3)..].Split(':')[0];
        var labels = host.Split('.');
        var wildcardLabels = labels.Count(l => l.Contains('*'));
        var wildcardIndex = Array.FindIndex(labels, l => l.Contains('*'));

        var safe = schemeEnd > 0
            && pattern.Count(c => c == '*') == 1
            && wildcardLabels == 1
            && labels[wildcardIndex] != "*"
            && labels.Length - wildcardIndex - 1 >= 2
            && labels.Skip(wildcardIndex + 1).All(l => l.Length > 0);

        if (!safe)
            throw new InvalidOperationException(
                $"Unsafe AllowedOriginPatterns entry '{pattern}'. Use one '*' inside a label that also has fixed text, " +
                "followed by at least two fixed labels, e.g. 'https://dashboard-*-team.vercel.app'.");
    }

    private static string Normalize(string origin) => origin.Trim().TrimEnd('/');
}
