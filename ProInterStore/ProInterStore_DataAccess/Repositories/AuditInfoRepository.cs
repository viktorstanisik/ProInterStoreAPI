using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProInterStore_DataAccess.Interfaces;
using ProInterStore_Domain;
using ProInterStore_Domain.EntityModels;
using ProInterStore_Shared.Enums;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProInterStore_DataAccess.Repositories
{
    public class AuditInfoRepository : IAuditInfoRepository
    {

        private readonly ProInterStoreDbContext _dbContext;

        public AuditInfoRepository(ProInterStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> CreateAudit(int userId, int auditEntityId, AuditEntityTypes entityType)
        {
            try
            {
                var newCreationAudit = await GetAuditInfoBasedOnEntityTypeAndSetEntityId(auditEntityId, entityType);

                    newCreationAudit.CreatedBy = userId;
                    newCreationAudit.RecordCreatedOn = DateTime.UtcNow;

                await _dbContext.AddAsync(newCreationAudit);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        public async Task<bool> DeleteAudit(int userId, int auditEntityId, AuditEntityTypes entityType)
        {
            try
            {
                var newCreationAudit = new AuditInfo();

                var auditInfoRecordToUpdate = await GetAuditInfoBasedOnEntityTypeAndSetEntityId(auditEntityId, entityType);

                    auditInfoRecordToUpdate.DeletedBy = userId;
                    auditInfoRecordToUpdate.RecordDeletedOn = DateTime.UtcNow;
                    
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        public async Task<bool> UpdateAudit(int userId, int auditEntityId, AuditEntityTypes entityType)
        {
            try
            {
                var auditInfoRecordToUpdate = await GetAuditInfoBasedOnEntityTypeAndSetEntityId(auditEntityId, entityType);

                    auditInfoRecordToUpdate.UpdatedBy = userId;
                    auditInfoRecordToUpdate.RecordUpdatedOn = DateTime.UtcNow;

                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        private async Task<AuditInfo> GetAuditInfoBasedOnEntityTypeAndSetEntityId(int auditEntityId, AuditEntityTypes entityType)
        {
            AuditInfo auditInfo = new();

            switch (entityType)
            {
                case AuditEntityTypes.StoreItemAudit:

                    auditInfo = (await _dbContext.AuditInfo
                                                 .Where(x => x.StoreItemId.Equals(auditEntityId) && !x.RecordDeletedOn.HasValue)
                                                 .OrderByDescending(x => x.CreatedBy)
                                                 .FirstOrDefaultAsync()) 
                                                 ?? auditInfo;

                    auditInfo.StoreItemId = auditEntityId;
                    break;

                default:
                    break;
            }
            return auditInfo;
        }
    }
}
