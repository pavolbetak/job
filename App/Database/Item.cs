namespace App.Database
{
    public class Item
    {
        public decimal Value {
            get
            {
                return Value;
            }
            set
            {
                Value = value;
                UpdateDate = DateTime.UtcNow;
            }
        }

        public DateTime UpdateDate { get; private set; }

        public Item(decimal value) 
        {
            UpdateDate = DateTime.UtcNow;
        }
    }
}
