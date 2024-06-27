namespace App.Database.Entity
{
    public class Author
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public virtual Image Image { get; set; }
        public virtual ICollection<Article> Articles { get; set; }
    }
}
