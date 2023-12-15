using ProInterStore_Domain.EntityModels;

namespace ProInterStore_DataAccess.Interfaces
{
    public interface IStoreItemRepository : IBaseRepository<StoreItem>
    {
        Task<StoreItem> GetStoreItemByItemCode(string itemCode);
    }
}
