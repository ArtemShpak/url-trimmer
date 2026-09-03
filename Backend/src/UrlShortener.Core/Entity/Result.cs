namespace UrlShortener.Core.Entity;

public class Result
{
    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public string? Error { get; }

    protected Result(bool isSuccess, string? error)
    {
        if (isSuccess && !string.IsNullOrEmpty(error))
            throw new InvalidOperationException("Успішний результат не може містити помилку.");

        if (!isSuccess && string.IsNullOrEmpty(error))
            throw new InvalidOperationException("Неуспішний результат має містити опис помилки.");

        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);

    public static Result Failure(string error) => new(false, error);
}

public class Result<T> : Result
{
    private readonly T? _value;

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Неможливо отримати значення для невдалого результату.");

    protected Result(bool isSuccess, T? value, string? error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    public static Result<T> Success(T value) => new(true, value, null);

    public new static Result<T> Failure(string error) => new(false, default, error);
}
