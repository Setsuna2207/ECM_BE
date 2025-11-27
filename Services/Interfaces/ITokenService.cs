using ECM_BE.Models.Entities;

namespace ECM_BE.Services.Interfaces
{
    public interface ITokenService
    {
        Task<string> createToken(User user);
    }
}
