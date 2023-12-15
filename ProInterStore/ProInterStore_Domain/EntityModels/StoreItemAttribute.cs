using ProInterStore_Domain.EntityModels;
using System.ComponentModel.DataAnnotations;

namespace ProInterStore_Domain.EntityModels
{
    public class StoreItemAttribute : BaseEntity
    {
        public string AttributeColor { get; set; }
        public int AttributeHeight { get; set; }
        public int AttributeWidth { get; set; }
        public int AttributeWeight { get; set; }
        public int StoreItemId { get; set; }
        public StoreItem StoreItem { get; set; }


    }
}
    