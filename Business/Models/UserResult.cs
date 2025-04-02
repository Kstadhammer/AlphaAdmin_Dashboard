using Domain.Models;

namespace Business.Models;

public class UserResult : ServiceResult
{
    public IEnumerable<User>? Result { get; set; }
}
