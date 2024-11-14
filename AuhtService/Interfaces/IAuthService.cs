namespace AuhtService.Interfaces
{
    public interface IAuthService
    {
        string GenerateToken(string userId, string userName);
    }
}