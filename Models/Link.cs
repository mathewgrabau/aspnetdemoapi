using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DemoApi.Models
{
	public class Link
	{
		public const string GetMethod = "GET";
		public const string PostMethod = "POST";

		public static Link To(string routeName, object routeValues = null)
		{
			return new Link { RouteName = routeName, RouteValues = routeValues, Method = GetMethod, Relations = null };
		}

		public static Link ToCollection(string routeName, object routeValues = null)
		{
			return new Link { RouteName = routeName, RouteValues = routeValues, Method = GetMethod, Relations = new string[] { "collection" } };
		}

		public static Link ToForm(string routeName, object routeValues = null, string method = PostMethod, params string[] relations)
		{
			return new Link
			{
				RouteName = routeName,
				RouteValues = routeValues,
				Method = method,
				Relations = relations
			};
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
		[JsonIgnore(Condition = JsonIgnoreCondition.Always)]
		public string RouteName { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.Always)]
		public object RouteValues { get; set; }
	}
}