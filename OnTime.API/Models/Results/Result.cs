namespace OnTime.API.Models.Results;

public class Result
{
  public bool IsSuccess { get; }
  public bool IsFailure => !IsSuccess;
  public Error Error { get; }

  protected Result()
  {
    Error = Error.None;
  }

  protected Result(bool isSuccess, Error error)
  {
    if (isSuccess && error != Error.None || !isSuccess && error == Error.None)
      throw new ArgumentException("Invalid combinations of parameters.");

    IsSuccess = isSuccess;
    Error = error;
  }

  public static Result Success() => new(true, Error.None);
  public static Result Failure(Error error) => new(false, error);

  public static Result<T> Success<T>(T value) => new(true, Error.None, value);
  public static Result<T> Failure<T>(Error error) => new(false, error, default);
}

public class Result<T> : Result
{
  public T Value => this.value ?? throw new InvalidOperationException("Result has no value. Did you forgot to check the sucess?");
  private readonly T? value;
  public Result(bool isSuccess, Error error, T? value) : base(isSuccess, error)
  {
    this.value = value;
  }
}
