using Microsoft.AspNetCore.Mvc;
using ProInterStore_Domain.DTOModels;
using ProInterStore_Service.Interfaces;
using ProInterStore_Service.Models;
using ProInterStore_Service.Services;
using Serilog;

namespace ProInterStore_Api.Controllers
{
    [TypeFilter(typeof(JWTInterceptorAuthFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class StoreItemController : ControllerBase
    {
        private readonly IStoreItemService _storeItemService;

        public StoreItemController(IStoreItemService storeItemService)
        {
            _storeItemService = storeItemService;
        }

        [HttpPost("create-store-item")]
        public async Task<IActionResult> CreateStoreItem(StoreItemDTO inputModel)
        {
            try
            {
                var data = await _storeItemService.CreateAsync(inputModel);

                return Ok(data);

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        [HttpPut("update-store-item")]
        public async Task<IActionResult> UpdateStoreItem(StoreItemDTO inputModel)
        {
            try
            {
                var data = await _storeItemService.UpdateAsync(inputModel);

                return Ok(data);

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        [HttpGet("get-all-store-items")]
        public async Task<List<StoreItemDTO>> GetAllStoreItems()
        {
            try
            {
                return await _storeItemService.GetAllAsync();

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        [HttpPost("deletes-tore-item")]
        public async Task<IActionResult> DeleteStoreItem(StoreItemDTO storeItem)
        {
            try
            {
                var deletedEntityId = await _storeItemService.DeleteAsync(storeItem);

                return Ok(deletedEntityId);

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }


        [HttpPost("filter-elements")]
        public async Task<List<StoreItemDTO>> FilterElements(IEnumerable<FilterElement> filters)
        {
            try
            {
                return await _storeItemService.FilterStoreItems(filters);

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

    }
}
