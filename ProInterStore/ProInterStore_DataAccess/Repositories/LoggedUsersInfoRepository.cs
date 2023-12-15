using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProInterStore_DataAccess.Interfaces;
using ProInterStore_Domain;
using ProInterStore_Domain.EntityModels;
using ProInterStore_Shared.AppConstans;
using ProInterStore_Shared.Enums;

namespace ProInterStore_DataAccess.Repositories
{
    public class LoggedUsersInfoRepository : ILoggedUsersInfoRepository
    {
        private readonly ProInterStoreDbContext _dbContext;
        private readonly ILogger<LoggedUsersInfoRepository> _logger;

        public LoggedUsersInfoRepository(ProInterStoreDbContext dbContext, ILogger<LoggedUsersInfoRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<LoggedUserInfo> CreateLoggedUserRecord(int userId)
        {
            try
            {
                var userisLogged = await _dbContext.LoggedUsersInfo.Where(x => x.UserId == userId && x.LoginStatus == (int)LoginStatus.LoggedIn)
                                                                   .OrderByDescending(x => x.LastLogin)
                                                                   .FirstOrDefaultAsync();
                if (userisLogged != null)
                {
                    userisLogged.LastLogin = DateTime.Now;
                    await _dbContext.SaveChangesAsync();
                    return userisLogged;
                }

                var loggedUser = new LoggedUserInfo()
                {
                    UserId = userId,
                    LastLogin = DateTime.Now,
                    LoginStatus = (int)LoginStatus.LoggedIn,
                };

                await _dbContext.AddAsync(loggedUser);
                await _dbContext.SaveChangesAsync();

                return loggedUser;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }

        }

        public async Task<LoggedUserInfo> LogoutUser(int userId)
        {
            try
            {
                var loggedUserInfo = await GetLoggedUserInfo(userId, true);

                if (loggedUserInfo is null) throw new Exception(ErrorMessages.GenericError);

                loggedUserInfo.LoginStatus = (int)LoginStatus.LoggedOut;
                await _dbContext.SaveChangesAsync();

                return loggedUserInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }

        }

        public async Task<LoggedUserInfo> GetLoggedUserInfo(int userId, bool isFromLogout = false)
        {
            try
            {
                return await _dbContext.LoggedUsersInfo
                                        .Where(x => x.UserId == userId && (!isFromLogout || x.LoginStatus == (int)LoginStatus.LoggedIn))
                                        .OrderByDescending(x => x.LastLogin)
                                        .FirstOrDefaultAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }

        }
    }
}
