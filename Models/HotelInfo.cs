using DemoApi.Infrastructure;
using System.Text.Json;

namespace DemoApi.Models
{
	public class HotelInfo : Resource, IEtaggable
	{
		public string Title { get; set; }

		public string Tagline { get; set; }

		public string Email { get; set; }

		public string Website { get; set; }

		public Address Location { get; set; }

		public string GetEtag()
		{
			var serialized = JsonSerializer.Serialize(this);
			return Md5Hash.ForString(serialized);
		}
    }
}