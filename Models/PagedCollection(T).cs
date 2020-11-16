using System.Text.Json.Serialization;

namespace DemoApi.Models
{
	public class PagedCollection<T> : Collection<T>
	{
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public int? Offset { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public int? Limit { get; set; }

		public int Size { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public Link First { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public Link Previous { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public Link Next { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public Link Last { get; set; }
	}
}