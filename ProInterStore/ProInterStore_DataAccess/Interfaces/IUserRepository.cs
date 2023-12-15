using ProInterStore_Domain.EntityModels;

namespace ProInterStore_DataAccess.Interfaces
{
    public interface IUserRepository
    {
        Task<User> CreateUser(User user);
        Task<User> GetUser(string email, string password);
        Task<User> GetUserByEmail(string email);
    }
}
