using Infrastructure.Middlewares;

namespace Meow;

public class MeowUserService : ICurrentUserService
{
    public string? UserName => "MeowUser";
}