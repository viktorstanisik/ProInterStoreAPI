using Microsoft.EntityFrameworkCore;
using ProInterStore_DataAccess.Interfaces;
using ProInterStore_Domain;
using ProInterStore_Domain.EntityModels;
using ProInterStore_Shared.AppConstans;
using Serilog;

namespace ProInterStore_DataAccess.Repositories
{
    public class StoreItemRepository : IStoreItemRepository
    {

        private readonly ProInterStoreDbContext _dbContext;

        public StoreItemRepository(ProInterStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> CreateAsync(StoreItem entity)
        {
            try
            {
                await _dbContext.AddAsync(entity);

                await _dbContext.SaveChangesAsync();


                return entity.Id;

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        public async Task<List<StoreItem>> GetAllAsync()
        {
            var storeItems =  await _dbContext.StoreItems
                                              .Where(x => !x.isDeleted)
                                              .Include(s => s.Attributes)
                                              .ToListAsync();

            return storeItems;
        }

        public async Task<StoreItem> GetByIdAsync(int id)
        {
            try
            {
                var storeItem = await _dbContext.StoreItems
                                                .Where(x => x.Id.Equals(id) && !x.isDeleted)
                                                .Include(s => s.Attributes)
                                                .FirstOrDefaultAsync();

                if (storeItem is null) throw new Exception(ErrorMessages.StoreItemNotFound);

                return storeItem;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        public async Task<int> DeleteAsync(StoreItem entity)
        {
            try
            {
                var storeItem = await _dbContext.StoreItems
                                                .FirstOrDefaultAsync(x => x.Id == entity.Id && !x.isDeleted);

                if (storeItem is null) throw new Exception(ErrorMessages.StoreItemNotFound);

                storeItem.isDeleted = true;

                await _dbContext.SaveChangesAsync();

                return storeItem.Id;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        public async Task<int> UpdateAsync(StoreItem entity)
        {
            //IUnitOFWork should be implemented -- 
            using var transaction = _dbContext.Database.BeginTransaction();

            try
            {
                var storeItem = await _dbContext.StoreItems
                                                .Include(at => at.Attributes)
                                                .FirstOrDefaultAsync(x => x.ItemCode == entity.ItemCode 
                                                                    && !x.isDeleted);

                if (storeItem is null) throw new Exception(ErrorMessages.StoreItemNotFound);

                storeItem.Name = entity.Name;
                storeItem.ProductGroupId = entity.ProductGroupId;   
                storeItem.UnitOfMeasure = entity.UnitOfMeasure;

                // Check if entity.Attributes is not null before manipulating the collection
                if (entity.Attributes != null)
                {
                    storeItem.Attributes.Clear();
                    storeItem.Attributes.AddRange(entity.Attributes);
                }

                await _dbContext.SaveChangesAsync();

                var entry = _dbContext.ChangeTracker.Entries().FirstOrDefault();
                var state = entry?.State;


                // Log or inspect validation errors

                transaction.Commit();

                return storeItem.Id;
            }
            catch (Exception ex)
            {
                transaction.Rollback();

                Log.Error(ex.Message);
                throw;
            }
        }

        public async Task<StoreItem> GetStoreItemByItemCode(string itemCode)
        {
            try
            {
                return await _dbContext.StoreItems
                                                .Where(x => x.ItemCode
                                                .Equals(itemCode) && !x.isDeleted)
                                                .FirstOrDefaultAsync();

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }
    }
}
