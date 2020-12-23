using System.Text.Json.Serialization;

namespace DemoApi.Models
{
    public class FormFieldOption
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Label { get; set; }

        public object Value { get; set; }
    }
}