namespace Infrastructure.Middlewares;

public interface ICurrentUserService
{
    string? UserName { get; }
}