namespace CleanTib.Application.Common.Models;

public class BaseResult
{
    public bool IsSuccess { get; }
    public string? ErrorMessage { get; }

    protected BaseResult(bool isSuccess, string? errorMessage = null)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }
}

public sealed class Result<T> : BaseResult
{
    public T Data { get; }

    private Result(bool isSuccess, T data, string? errorMessage = null)
        : base(isSuccess, errorMessage!)
    {
        Data = data;
    }

    public static Result<T> Success(T data)
    {
        return new Result<T>(true, data, null);
    }

    public static Result<T> Failure(string errorMessage)
    {
        return new Result<T>(false, default!, errorMessage);
    }
}

public sealed class Result : BaseResult
{
    private Result(bool isSuccess, string? errorMessage = null)
        : base(isSuccess, errorMessage!)
    {
    }

    public static Result Success()
    {
        return new Result(true);
    }

    public static Result Failure(string errorMessage)
    {
        return new Result(false, errorMessage);
    }
}