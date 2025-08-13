using Core.DTOs.Item;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepository;

        public ItemService(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        public async Task<ItemSearchResponse> SearchItemsAsync(ItemSearchRequest request)
        {
            return await _itemRepository.SearchItemsAsync(request);
        }

        public async Task<ItemDto> GetItemByCodeAsync(string itemCode)
        {
            return await _itemRepository.GetItemByCodeAsync(itemCode);
        }

        public async Task<List<ItemAutocompleteDto>> GetItemsAutocompleteAsync(string term)
        {
            return await _itemRepository.GetItemsAutocompleteAsync(term);
        }

        public async Task<ItemWarehouseStockResponse> GetItemStockByWarehousesAsync(string itemCode)
        {
            return await _itemRepository.GetItemStockByWarehousesAsync(itemCode);
        }
    }
}
