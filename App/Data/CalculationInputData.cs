using System.ComponentModel.DataAnnotations;

namespace App.Data
{
    public class CalculationInputData
    {
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Input { get; set; }
    }
}
