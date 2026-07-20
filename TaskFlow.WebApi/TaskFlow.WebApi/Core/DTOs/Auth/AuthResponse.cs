namespace TaskFlow.WebApi.Core.DTOs.Auth
{
    public class AuthResponse
    {
        public required string Token { get; set; } 
        public required DateTime ExpireAt { get; set; }
        public required Guid UserId { get; set; }
        public required string UserName { get; set; }
        public required string Email { get; set; }


    }
}
