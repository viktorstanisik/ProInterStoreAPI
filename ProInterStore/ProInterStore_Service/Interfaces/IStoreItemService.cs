using ProInterStore_Domain.DTOModels;
using ProInterStore_Service.Models;

namespace ProInterStore_Service.Interfaces
{
    public interface IStoreItemService : IBaseService<StoreItemDTO>
    {
        Task<List<StoreItemDTO>> FilterStoreItems(IEnumerable<FilterElement> filters);
    }
}
