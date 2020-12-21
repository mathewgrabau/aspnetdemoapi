using DemoApi.Infrastructure;

namespace DemoApi.Models
{
	// Represents the resource that is presented (note the rate converted from cents to a decimal)
	// Splitting the objects give us flexibility
	public class Room : Resource
	{
		[Sortable]
		[Searchable]
		public string Name { get; set; }

		[Sortable(Default = true)]
		[Searchable]
		public decimal Rate { get; set; }
	}
}