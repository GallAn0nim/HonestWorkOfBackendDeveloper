using System.Diagnostics.CodeAnalysis;

namespace HonestWork.Api.Shared;

public class Result<T>
{
    private Result(T? value, ValidationError? error)
    {
        Value = value;
        Error = error;
    }

    public T? Value { get; }
    public ValidationError? Error { get; }

    [MemberNotNullWhen(true, nameof(Error))]
    [MemberNotNullWhen(false, nameof(Value))]
    public bool IsFailure => Error is not null;

    public static implicit operator Result<T>(T value)
    {
        return new(value, null);
    }

    public static implicit operator Result<T>(ValidationError error)
    {
        return new(default, error);
    }
}
