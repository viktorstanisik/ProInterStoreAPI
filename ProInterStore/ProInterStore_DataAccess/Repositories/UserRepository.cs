using Microsoft.EntityFrameworkCore;
using ProInterStore_DataAccess.Interfaces;
using ProInterStore_Domain;
using ProInterStore_Domain.EntityModels;
using ProInterStore_Shared.AppConstans;
using Serilog;

namespace ProInterStore_DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ProInterStoreDbContext _dbContext;

        public UserRepository(ProInterStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> CreateUser(User userModel)
        {
            try
            {
                await _dbContext.AddAsync(userModel);
                await _dbContext.SaveChangesAsync();

                return userModel;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        public async Task<User> GetUser(string email, string password)
        {
            try
            {
                return await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email && x.Password == password);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        public async Task<User> GetUserByEmail(string email)
        {
            try
            {
                return await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }



    }
}
