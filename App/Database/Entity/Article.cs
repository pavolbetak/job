namespace App.Database.Entity
{
    public class Article
    {
        public long Id { get; set; } // Primary key
        public string Title { get; set; } // Index
        public virtual ICollection<Author> Authors { get; set; } // Many-To-Many
        public virtual Site Site { get; set; } // One-To-Many
    }
}
