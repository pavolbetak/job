namespace App.Database
{
    public class Storage
    {
        private readonly IDictionary<int, Item> _items;

        public Storage()
        {
                _items = new Dictionary<int, Item>();
        }

        public Item? GetItem(int key)
        {
            return _items.ContainsKey(key) ? _items[key] : null;
        }

        public void AddOrUpdateNewItem(int key, Item item)
        {
            if (_items.ContainsKey(key))
            {
                _items.Remove(key);
            }

            _items.Add(key, item);
        }
    }
}
