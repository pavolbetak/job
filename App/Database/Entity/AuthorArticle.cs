namespace App.Database.Entity
{
    public class AuthorArticle
    {
        public long AuthorId { get; set; }
        public Author Author { get; set; }

        public long ArticleId { get; set; }
        public Article Article { get; set; }
    }
}
