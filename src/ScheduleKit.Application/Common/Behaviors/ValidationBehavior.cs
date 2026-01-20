using FluentValidation;
using MediatR;
using ScheduleKit.Domain.Common;

namespace ScheduleKit.Application.Common.Behaviors;

/// <summary>
/// Pipeline behavior that validates requests using FluentValidation before they reach the handler.
/// </summary>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
        {
            var errorMessage = string.Join("; ", failures.Select(f => f.ErrorMessage));

            // Create appropriate failure result based on TResponse type
            if (typeof(TResponse).IsGenericType)
            {
                var resultType = typeof(TResponse).GetGenericArguments()[0];
                var method = typeof(Result).GetMethod(nameof(Result.Failure), new[] { typeof(string) });
                var genericMethod = method!.MakeGenericMethod(resultType);
                return (TResponse)genericMethod.Invoke(null, new object[] { errorMessage })!;
            }

            return (TResponse)(object)Result.Failure(errorMessage);
        }

        return await next();
    }
}
