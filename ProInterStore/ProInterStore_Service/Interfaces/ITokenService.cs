using ProInterStore_Service.Models;

namespace ProInterStore_Service.Interfaces
{
    public interface ITokenService
    {
        string GenerateJwtToken(LoginModel model, int id);
    }
}
