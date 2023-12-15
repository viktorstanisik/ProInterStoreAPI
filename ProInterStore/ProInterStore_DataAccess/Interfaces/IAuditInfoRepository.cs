using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ProInterStore_Domain.EntityModels;
using ProInterStore_Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProInterStore_DataAccess.Interfaces
{
    public interface IAuditInfoRepository
    {
        Task<bool> CreateAudit(int userId, int auditEntityId, AuditEntityTypes entityType);
        Task<bool> DeleteAudit(int userId, int auditEntityId, AuditEntityTypes entityType);
        Task<bool> UpdateAudit(int userId, int auditEntityId, AuditEntityTypes entityType);

    }
}
