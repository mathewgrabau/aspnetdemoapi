using Newtonsoft.Json;

namespace DemoApi.Models
{
	public abstract class Resource
	{
		[JsonProperty(Order = -2)]  // Make it at the very top (nice touch for human readability)
		public string Href { get; set; }    // RESTFUL way of expressing an ID for the resource
	}
}