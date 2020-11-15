using System.Text.Json.Serialization;

namespace DemoApi.Models
{
	public abstract class Resource : Link
	{
		[JsonIgnore]
		public Link Self { get; set; }    // RESTFUL way of expressing an ID for the resource
	}
}