namespace App.Database.Entity
{
    public class Article
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public virtual ICollection<Author> Authors { get; set; }
        public virtual Site Site { get; set; }
    }
}
