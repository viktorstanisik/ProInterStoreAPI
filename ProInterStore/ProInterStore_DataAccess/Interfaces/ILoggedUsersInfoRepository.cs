using ProInterStore_Domain.EntityModels;

namespace ProInterStore_DataAccess.Interfaces
{
    public interface ILoggedUsersInfoRepository
    {
        Task<LoggedUserInfo> CreateLoggedUserRecord(int userId);
        Task<LoggedUserInfo> GetLoggedUserInfo(int userId, bool isFromLogout = false);
        Task<LoggedUserInfo> LogoutUser(int userId);

    }
}
