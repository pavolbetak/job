using System.Text.Json.Serialization;

namespace App.Data
{
    public class CalculatedData
    {
        [JsonPropertyName("previous_value")]
        public decimal? PreviousValue{ get; set; }

        [JsonPropertyName("computed_value")]
        public decimal ComputedValue{ get; set; }

        [JsonPropertyName("input_value")]
        public decimal InputValue { get; set; }
    }
}
