using System.Text.RegularExpressions;
using ScheduleKit.Domain.Common;

namespace ScheduleKit.Domain.ValueObjects;

/// <summary>
/// Represents a validated email address.
/// </summary>
public sealed partial record Email
{
    public string Value { get; }

    private Email(string value)
    {
        Value = value.ToLowerInvariant().Trim();
    }

    public static Result<Email> Create(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Result.Failure<Email>("Email address is required.");

        email = email.Trim();

        if (email.Length > 254)
            return Result.Failure<Email>("Email address is too long.");

        if (!EmailRegex().IsMatch(email))
            return Result.Failure<Email>("Invalid email address format.");

        return Result.Success(new Email(email));
    }

    /// <summary>
    /// Creates an Email without validation. Use only when loading from database.
    /// </summary>
    public static Email FromString(string email) => new(email);

    public override string ToString() => Value;

    [GeneratedRegex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.Compiled)]
    private static partial Regex EmailRegex();
}
