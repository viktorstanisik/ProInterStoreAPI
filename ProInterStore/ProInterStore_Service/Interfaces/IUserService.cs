using ProInterStore_Domain.DTOModels;
using ProInterStore_Domain.EntityModels;
using ProInterStore_Service.Models;

namespace ProInterStore_Service.Interfaces
{
    public interface IUserService
    {
        Task<int> Create(UserDto userDtoModel);
        Task<JwtResponseModel> Login(LoginModel userDtoModel);
        int GetLoggedUserIdFromHttpContext();
        Task<bool> LogoutUser();
        Task<LoggedUserInfo> GetUserByUserIdFromLoggedUserInfo(int userId);

    }
}
