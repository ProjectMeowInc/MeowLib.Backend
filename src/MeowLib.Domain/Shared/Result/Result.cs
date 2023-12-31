namespace MeowLib.Domain.Shared.Result;

public class Result
{
    private readonly Exception? _error;

    protected Result()
    {
        _error = default;
    }

    protected Result(Exception error)
    {
        _error = error;
    }

    public bool IsFailure => _error is not null;

    public Exception GetError()
    {
        if (!IsFailure)
        {
            throw new NotImplementedException();
        }

        return _error ?? throw new NullReferenceException();
    }

    public static Result Ok()
    {
        return new Result();
    }

    public static Result Fail(Exception error)
    {
        return new Result(error);
    }
}

public class Result<TResult>
{
    private readonly Exception? _error;
    private readonly TResult? _result;

    protected Result(TResult result)
    {
        _result = result;
        _error = default;
    }

    protected Result(Exception error)
    {
        _result = default;
        _error = error;
    }

    public bool IsFailure => _error is not null;

    public TResult GetResult()
    {
        if (IsFailure)
        {
            throw new NotImplementedException();
        }

        return _result ?? throw new NullReferenceException();
    }

    public Exception GetError()
    {
        if (!IsFailure)
        {
            throw new NotImplementedException();
        }

        return _error ?? throw new NullReferenceException();
    }

    public static Result<TResult> Ok(TResult result)
    {
        return new Result<TResult>(result);
    }

    public static Result<TResult> Fail(Exception error)
    {
        return new Result<TResult>(error);
    }

    public static implicit operator Result<TResult>(TResult data)
    {
        return Ok(data);
    }
}