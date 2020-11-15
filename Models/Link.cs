using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DemoApi.Models
{
	public class Link
	{
		public const string GetMethod = "GET";

		public static Link To(string routeName, object routeValues = null)
		{
			return new Link { RouteName = routeName, RouteValues = routeValues, Method = GetMethod, Relations = null };
		}

		//[JsonProperty(Order = -4)]
		public string Href { get; set; }

		[JsonPropertyName("rel")]
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		//[JsonPropertyName(Order = -3, PropertyName = "rel", NullValueHandling = NullValueHandling.Ignore)]
		public string[] Relations { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault | JsonIgnoreCondition.WhenWritingNull)]
		//[JsonPropertyName(Order = -2, DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
		[DefaultValue(GetMethod)]   // Based on the ION specification (unspecified = default = GET)
		public string Method { get; set; }

		// Stashing the values to use in filter to generate the URL (link rewriting filter)
		// They are just temporary (hence the JsonIgnore)
		[JsonIgnore]
		public string RouteName { get; set; }

		[JsonIgnore]
		public object RouteValues { get; set; }
	}
}