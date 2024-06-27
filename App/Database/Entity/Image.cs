namespace App.Database.Entity
{
    public class Image
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public Author Author { get; set; }
    }
}
