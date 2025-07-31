using Core.DTOs.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Services
{
    public interface IItemService
    {
        Task<ItemSearchResponse> SearchItemsAsync(ItemSearchRequest request);
        Task<ItemDto> GetItemByCodeAsync(string itemCode);
        Task<List<ItemAutocompleteDto>> GetItemsAutocompleteAsync(string term);
    }
}
