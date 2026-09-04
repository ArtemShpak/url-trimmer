namespace UrlShortener.Core.Entity;

public class Result
{
    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error.Error? Error { get; }

    protected Result(bool isSuccess, Error.Error? error)
    {
        if (isSuccess && error is not null)
            throw new InvalidOperationException("A successful result cannot contain an error.");
        if (!isSuccess && error is null)
            throw new InvalidOperationException("A failure result must contain an error description.");
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);

    public static Result Failure(Error.Error error) => new(false, error);
}

public class Result<T> : Result
{
    private readonly T? _value;

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Неможливо отримати значення для невдалого результату.");

    protected Result(bool isSuccess, T? value, Error.Error? error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    public static Result<T> Success(T value) => new(true, value, null);

    public new static Result<T> Failure(Error.Error error) => new(false, default, error);
}
