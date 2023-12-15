using ProInterStore_Domain.EntityModels;
using System.ComponentModel.DataAnnotations;

namespace ProInterStore_Domain.EntityModels
{
    public class StoreItem : BaseEntity
    {

        [Required]
        public string ItemCode { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int ProductGroupId { get; set; }

        [Required]
        public string UnitOfMeasure { get; set; }

        public bool isDeleted { get; set; }

        public List<StoreItemAttribute> Attributes { get; set; }
    }
}
