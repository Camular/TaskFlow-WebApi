using TaskFlow.WebApi.Core.DTOs.Auth;

namespace TaskFlow.WebApi.Core.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
    }
}
