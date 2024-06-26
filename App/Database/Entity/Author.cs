namespace App.Database.Entity
{
    public class Author
    {
        public long Id { get; set; } // Primary key
        public string Name { get; set; } // Unique index
        public virtual Image Image { get; set; } // One-To-One
        public virtual ICollection<Article> Articles { get; set; }
    }
}
