using App.Data;
using App.RabbitMq;
using Microsoft.Extensions.Caching.Memory;

namespace App.Services
{
    public class StorageService
    {
        private IMemoryCache _cache;

        private MessageBroker _messageBroker;

        public StorageService(IMemoryCache cache, MessageBroker messageBroker)
        {
            _cache = cache;
            _messageBroker = messageBroker;
        }

        public CalculatedData SaveDataIntoStorageAndNotify(int key, decimal inputValue)
        {
            var calculatedData = CalculatedAndStoreData(key, inputValue);

            _messageBroker.PublishCalculationData(calculatedData);

            return calculatedData;
        }

        private CalculatedData CalculatedAndStoreData(int key, decimal inputValue)
        {
            const decimal defaultItemValue = 2;
            const double maxOldnestSeconds = 15;

            //  not recommended storing user data input cache => unpredictable amount of memery
            if (!_cache.TryGetValue(key, out TimestampValue data))
            {
                // should have expiration date/time, depends on data behaviour
                _cache.Set(key, new TimestampValue{
                    Timestamp = DateTime.UtcNow,
                    Value = defaultItemValue
                });

                return new CalculatedData { PreviousValue = null, ComputedValue = defaultItemValue, InputValue = inputValue };
            }

            if(DateTime.UtcNow - data.Timestamp > TimeSpan.FromSeconds(maxOldnestSeconds))
            {
                var newValue = new TimestampValue { Timestamp = DateTime.UtcNow, Value = defaultItemValue };

                _cache.Set(key, newValue);

                return new CalculatedData { PreviousValue = data.Value, ComputedValue = newValue.Value, InputValue = inputValue };
            }

            // Can lost precision (double)value
            // TODO: Exception decimal => double
            var calculatedValue = Math.Pow(Math.Log((double)inputValue), 3);
            var newCalculatedValue = new TimestampValue { Timestamp = DateTime.UtcNow, Value = (decimal)calculatedValue };

            _cache.Set(key, newCalculatedValue);

            return new CalculatedData { PreviousValue = data.Value, ComputedValue = newCalculatedValue.Value, InputValue = inputValue };
        }
    }

    public record TimestampValue
    {
        public DateTime Timestamp { get; init; }

        public decimal Value { get; init; }
    }
}
