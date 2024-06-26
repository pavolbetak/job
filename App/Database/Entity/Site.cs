namespace App.Database.Entity
{
    public class Site
    {
        public long Id { get; set; } // Primary key
        public DateTimeOffset CreatedAt { get; set; }
        public virtual ICollection<Article> Articles { get; set; }
    }
}
