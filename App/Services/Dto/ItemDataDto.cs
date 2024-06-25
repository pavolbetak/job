namespace App.Services.Dto
{
    public class ItemDataDto
    {
        public int Key {  get; set; }
        public DateTime? PreviousDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public decimal Value { get; set; }
    }
}
