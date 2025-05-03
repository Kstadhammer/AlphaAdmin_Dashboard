namespace Domain.Models.Base;

public class BaseResult
{
    public bool Succeeded { get; set; }
    public int StatusCode { get; set; } = 200;
    public string Error { get; set; } = null!;
}

public class BaseResult<T> : BaseResult
{
    public T? Result { get; set; }
}
