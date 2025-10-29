using Infrastructure.Middlewares;

namespace API.Middlewares;

public class CurrentUserService(IHttpContextAccessor accessor) : ICurrentUserService
{
    public string? UserName => accessor?.HttpContext?.User?.Identity?.Name;
}