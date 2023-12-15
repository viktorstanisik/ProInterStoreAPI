using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProInterStore_Domain.EntityModels
{
    public class AuditInfo : BaseEntity
    {
        public int? StoreItemId { get; set; }
        public StoreItem StoreItem { get; set; }
        public DateTime? RecordCreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? RecordUpdatedOn { get; set; }

        public DateTime? RecordDeletedOn { get; set; }
        public int? DeletedBy { get; set; }

    }
}
