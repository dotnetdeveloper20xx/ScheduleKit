using System.Text.RegularExpressions;
using ScheduleKit.Domain.Common;

namespace ScheduleKit.Domain.ValueObjects;

/// <summary>
/// Represents a URL-friendly identifier (slug).
/// Used for public-facing URLs like /book/john-doe/30-min-consultation
/// </summary>
public sealed partial record Slug
{
    public const int MinLength = 3;
    public const int MaxLength = 100;

    public string Value { get; }

    private Slug(string value)
    {
        Value = value;
    }

    public static Result<Slug> Create(string? slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return Result.Failure<Slug>("Slug is required.");

        slug = slug.Trim().ToLowerInvariant();

        if (slug.Length < MinLength)
            return Result.Failure<Slug>($"Slug must be at least {MinLength} characters.");

        if (slug.Length > MaxLength)
            return Result.Failure<Slug>($"Slug cannot exceed {MaxLength} characters.");

        if (!SlugRegex().IsMatch(slug))
            return Result.Failure<Slug>("Slug can only contain lowercase letters, numbers, and hyphens.");

        if (slug.StartsWith('-') || slug.EndsWith('-'))
            return Result.Failure<Slug>("Slug cannot start or end with a hyphen.");

        if (slug.Contains("--"))
            return Result.Failure<Slug>("Slug cannot contain consecutive hyphens.");

        return Result.Success(new Slug(slug));
    }

    /// <summary>
    /// Generates a slug from a name or title.
    /// </summary>
    public static Result<Slug> FromName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Slug>("Name is required to generate slug.");

        var slug = name
            .ToLowerInvariant()
            .Trim()
            .Replace(" ", "-")
            .Replace("_", "-");

        // Remove invalid characters
        slug = InvalidCharsRegex().Replace(slug, "");

        // Remove consecutive hyphens
        slug = MultipleHyphensRegex().Replace(slug, "-");

        // Trim hyphens from ends
        slug = slug.Trim('-');

        if (slug.Length < MinLength)
            return Result.Failure<Slug>($"Generated slug is too short. Original name: {name}");

        if (slug.Length > MaxLength)
            slug = slug[..MaxLength].TrimEnd('-');

        return Create(slug);
    }

    /// <summary>
    /// Creates a Slug without validation. Use only when loading from database.
    /// </summary>
    public static Slug FromString(string slug) => new(slug);

    public override string ToString() => Value;

    [GeneratedRegex(@"^[a-z0-9]+(-[a-z0-9]+)*$", RegexOptions.Compiled)]
    private static partial Regex SlugRegex();

    [GeneratedRegex(@"[^a-z0-9\-]", RegexOptions.Compiled)]
    private static partial Regex InvalidCharsRegex();

    [GeneratedRegex(@"-+", RegexOptions.Compiled)]
    private static partial Regex MultipleHyphensRegex();
}
