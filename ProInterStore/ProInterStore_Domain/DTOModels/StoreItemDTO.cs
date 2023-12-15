using ProInterStore_Domain.EntityModels;

namespace ProInterStore_Domain.DTOModels
{
    public class StoreItemDTO
    {
        public int Id { get; set; }
        public string ItemCode { get; set; }
        public string Name { get; set; }
        public int ProductGroupId { get; set; }
        public string UnitOfMeasure { get; set; }
        public List<StoreItemAttributeDTO> Attributes { get; set; }

    }
}
