using App.Calculation;
using App.Data;
using App.Database;
using App.Services.Dto;

namespace App.Services
{
    public class StorageService
    {
        private Storage _storage;

        private Calculator _calculator;

        public StorageService(Storage storage, Calculator calculator)
        {
            _storage = storage;
            _calculator = calculator;
        }

        public ItemDataDto SaveDataIntoStorage(ItemDataDto item)
        {
            item.PreviousDate = GetUpdateDateForItem(item.Key);
            item.Value = AddItemIntoStorage(item.Key, item);
            item.UpdateDate = GetUpdateDateForItem(item.Key);
            
            return item;
        }

        private decimal AddItemIntoStorage(int key, ItemDataDto item)
        {
            const double oldItemTreshold = 15;
            const decimal defaultItemValue = 2;

            var existingItem = _storage.GetItem(key);
            if (existingItem == null || existingItem.UpdateDate > DateTime.UtcNow.AddSeconds(oldItemTreshold))
            {
                _storage.AddOrUpdateNewItem(key, new Item(defaultItemValue));

                return defaultItemValue;
            }
            else
            {
                var calculatedItemValue = (decimal)_calculator.CalculateItemData((double)item.Value);

                _storage.AddOrUpdateNewItem(key, new Item(calculatedItemValue));

                return calculatedItemValue;
            }
        }

        private DateTime? GetUpdateDateForItem(int key)
        {
            var item = _storage.GetItem(key);

            return item != null ? item.UpdateDate : null;
        }
    }
}
