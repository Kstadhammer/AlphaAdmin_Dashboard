using Domain.Models;

namespace Business.Models;

public class AuthResult : ServiceResult { }

public class AuthResult<T> : ServiceResult
{
    public T? Result { get; set; }
}
