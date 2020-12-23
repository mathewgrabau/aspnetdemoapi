using System.Text.Json.Serialization;

namespace DemoApi.Models
{
    public class FormField
    {
        public string Name { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Label { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public FormFieldOption[] Options { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Pattern { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool Required { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool Secret { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault | JsonIgnoreCondition.WhenWritingNull)]
        public string Type { get; set; } = DefaultType;
        public const string DefaultType = "string";

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? MinLength { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? MaxLength { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object Value { get; set; }
    }
}