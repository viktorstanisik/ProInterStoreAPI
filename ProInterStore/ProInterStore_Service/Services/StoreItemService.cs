using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using ProInterStore_DataAccess.Interfaces;
using ProInterStore_DataAccess.Repositories;
using ProInterStore_Domain.DTOModels;
using ProInterStore_Domain.EntityModels;
using ProInterStore_Service.Interfaces;
using ProInterStore_Service.Models;
using ProInterStore_Shared.AppConstans;
using ProInterStore_Shared.Enums;
using ProInterStore_Shared.HelperMethods;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProInterStore_Service.Services
{
    public class StoreItemService : IStoreItemService
    {
        private readonly IStoreItemRepository _storeItemRepository;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IAuditInfoRepository _auditInfoRepository;
        private readonly IConfiguration _configuration;

        public StoreItemService(IStoreItemRepository storeItemRepository, IMapper mapper, IUserService userService, IConfiguration configuration, IAuditInfoRepository auditInfoRepository)
        {
            _storeItemRepository = storeItemRepository;
            _mapper = mapper;
            _userService = userService;
            _configuration = configuration;
            _auditInfoRepository = auditInfoRepository;
        }

        public async Task<int> CreateAsync(StoreItemDTO entity)
        {
            await ValidateInputFields(entity);

            var createdStoreItemId = await _storeItemRepository.CreateAsync(_mapper.Map<StoreItem>(entity));

            await _auditInfoRepository.CreateAudit(_userService.GetLoggedUserIdFromHttpContext(),
                                                   createdStoreItemId,
                                                   AuditEntityTypes.StoreItemAudit);

            return createdStoreItemId;
        }

        public async Task<int> DeleteAsync(StoreItemDTO storeItemDto)
        {

            var deletedStoreItemId = await _storeItemRepository.DeleteAsync(_mapper.Map<StoreItem>(storeItemDto));

            await _auditInfoRepository.DeleteAudit(_userService.GetLoggedUserIdFromHttpContext(),
                                                   deletedStoreItemId,
                                                   AuditEntityTypes.StoreItemAudit);

            return deletedStoreItemId;

        }

        public async Task<List<StoreItemDTO>> GetAllAsync()
        {
            var storeItemList = await _storeItemRepository.GetAllAsync();
            var storeItemListDTO = _mapper.Map<List<StoreItemDTO>>(storeItemList);


            return storeItemListDTO;
        }

        public async Task<StoreItemDTO> GetByIdAsync(int id)
        {
            var storeItemDb = await _storeItemRepository.GetByIdAsync(id);

            return _mapper.Map<StoreItemDTO>(storeItemDb);
        }

        public async Task<int> UpdateAsync(StoreItemDTO entity)
        {
            await ValidateInputFields(entity, true);

            var updatedStoreItemId = await _storeItemRepository.UpdateAsync(_mapper.Map<StoreItem>(entity));

            await _auditInfoRepository.UpdateAudit(_userService.GetLoggedUserIdFromHttpContext(),
                                                   updatedStoreItemId,
                                                   AuditEntityTypes.StoreItemAudit);

            return updatedStoreItemId;
        }

        // The Filter method applies dynamic filtering on a list of StoreItemDTO entities based on a provided set of FilterElements.
        // It first retrieves all StoreItemDTO entities, then iterates through the filters, sorting them to ensure StoreItemDTO filters are processed first.
        // For each filter, it determines whether it is a filter for StoreItemDTO or StoreItemAttributeDTO, applies the corresponding filtering logic,
        // and ultimately returns a list of filtered StoreItemDTO entities. The result is then mapped to a list of StoreItemDTO using AutoMapper.
        public async Task<List<StoreItemDTO>> FilterStoreItems(IEnumerable<FilterElement> filters)
        {
            var storeItemList = await _storeItemRepository.GetAllAsync();
            var filteredList = new List<ProInterStore_Domain.EntityModels.StoreItem>();

            // In order to have dynamic filters, we need to make sure that we will first filter StoreItemDTO and after that we will filter the child entities Attributes
            // and join the child entities with the Parent one and return a list with filtered ones.
            var sortedFilters = SortFilters(filters);

            foreach (var filter in sortedFilters)
            {
                filter.TrimStringProperties();

                var storeItemProperty = typeof(StoreItemDTO).GetProperty(filter.PropertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                var storeItemAttribute = typeof(StoreItemAttributeDTO).GetProperty(filter.PropertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                // If the filter is not for StoreItemAttributeDTO
                if (storeItemAttribute == null)
                {
                    // Apply filter will return a List with all of the elements that have the current filter value
                    filteredList = ApplyFilter(storeItemList, storeItemProperty, filter);
                }
                else
                {
                    // Selecting the storeItemAttribute from the storeItemProperty in order to apply a filter also for them
                    var allAttributes = filteredList.SelectMany(x => x.Attributes).ToList();
                    var filteredAttributes = ApplyFilter(allAttributes, storeItemAttribute, filter);

                    // Selecting only the StoreItemAttributeDTO that has the matching storeItemAttribute
                    var storeItemIdsToCheck = filteredAttributes.Select(x => x.StoreItemId).ToList();

                    filteredList = filteredList.Where(item => storeItemIdsToCheck.Contains(item.Id)).ToList();
                }
            }
            filteredList = filteredList
                            .OrderByDescending(x => x.Id)
                            .ThenBy(x => x.ItemCode)
                            .Take(10)
                            .ToList();

            return _mapper.Map<List<StoreItemDTO>>(filteredList);
        }

        // The SortFilters method separates filters into two lists: storeItemFilters and attributeFilters.
        // It ignores filters with empty or null PropertyName.
        // The resulting IEnumerable is a concatenation of storeItemFilters followed by attributeFilters, ensuring that attributeFilters are processed last.
        private IEnumerable<FilterElement> SortFilters(IEnumerable<FilterElement> filters)
        {
            var storeItemFilters = new List<FilterElement>();
            var attributeFilters = new List<FilterElement>();

            foreach (var filter in filters)
            {
                if (string.IsNullOrWhiteSpace(filter.PropertyName))
                {
                    continue;
                }

                if (typeof(StoreItemDTO).GetProperty(filter.PropertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) != null)
                {
                    storeItemFilters.Add(filter);
                }
                else if (typeof(StoreItemAttributeDTO).GetProperty(filter.PropertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) != null)
                {
                    attributeFilters.Add(filter);
                }
            }

            // Concatenate the two lists, but place attributeFilters last
            return storeItemFilters.Concat(attributeFilters.OrderBy(f => f.PropertyName != "StoreItemAttributeDTO"));
        }

        // ApplyFilter method filters a list of items based on a specified property and filter conditions.
        // It returns a filtered list of items.
        // It uses reflection to dynamically access the property and performs the filtering.
        // The result is a filtered list of entities that match the given property and filter value.
        public List<T> ApplyFilter<T>(List<T> itemList, PropertyInfo propertyToFilter, FilterElement filter)
        {
            return itemList
                .Where(item =>
                {
                    var itemProperties = item.GetType().GetProperties().ToList();
                    var currentItemProperty = itemProperties.FirstOrDefault(x => propertyToFilter.Name.Equals(x.Name));

                    if (currentItemProperty == null)
                    {
                        // Handle the case where the propertyToFilter does not exist on the current item
                        return false;
                    }

                    var itemPropertyValue = currentItemProperty.GetValue(item, null);

                    // Perform a null check
                    if (itemPropertyValue == null || filter.FilterValue == null)
                    {
                        return object.Equals(itemPropertyValue, filter.FilterValue);
                    }

                    // Check if the types are compatible for comparison
                    if (itemPropertyValue.GetType().IsAssignableFrom(filter.FilterValue.GetType()))
                    {
                        // Perform a type-safe comparison
                        return itemPropertyValue.Equals(filter.FilterValue);
                    }

                    // Check if both values are numeric and can be compared
                    if (IsNumeric(itemPropertyValue) && IsNumeric(filter.FilterValue))
                    {
                        return Convert.ToDouble(itemPropertyValue) == Convert.ToDouble(filter.FilterValue);
                    }

                    return false;
                })
                .ToList();
        }

        // The IsNumeric method checks if an object is of a numeric type or a string representing a numeric value.
        // It returns true if the value is numeric, and false otherwise.
        private bool IsNumeric(object value)
        {
            return value is byte || value is sbyte || value is short || value is ushort ||
                   value is int || value is uint || value is long || value is ulong ||
                   value is float || value is double || value is decimal || value is string;
        }
        private async Task<bool> ValidateInputFields(StoreItemDTO storeItemModel, bool validationIsForUpdatingStoreItem = false)
        {
            storeItemModel.TrimStringProperties();

            Regex genericOnlyLetterAndNumbersRegex = new Regex(_configuration["RegexValidation:GenericValidationOnlyNumberAndLetterRegex"]);
            Regex genericValidationOnlyLetterRegex = new Regex(_configuration["RegexValidation:GenericValidationOnlyLetterRegex"]);


            if (!genericOnlyLetterAndNumbersRegex.IsMatch(storeItemModel.ItemCode)
                || !genericValidationOnlyLetterRegex.IsMatch(storeItemModel.Name)
                || !genericValidationOnlyLetterRegex.IsMatch(storeItemModel.UnitOfMeasure))
                throw new InvalidDataException(ErrorMessages.InvalidStoreItem);

            //Check if we are passing valid Product Group Id from the frontend
            if (!Enum.IsDefined(typeof(ProductGroup), storeItemModel.ProductGroupId)) throw new InvalidDataException(ErrorMessages.InvalidProductGroup);

            var existingStoreItem = await _storeItemRepository.GetStoreItemByItemCode(storeItemModel.ItemCode);

            //if we create a new store Item Code Id needs to be unique
            if (!validationIsForUpdatingStoreItem && existingStoreItem is not null) throw new Exception(ErrorMessages.StoreItemExist);


            //if we update a  store Item we need to find entity with that Code Id
            if (validationIsForUpdatingStoreItem && existingStoreItem is null) throw new Exception(ErrorMessages.InvalidStoreItem);

            return await Task.FromResult(true);
        }

    }
}
