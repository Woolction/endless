using Domain.Entities;

namespace Domain.Common.Interfaces.Services;

public interface IAuthService
{
    Task<string[]> CreateTokenResponse(User user);
}