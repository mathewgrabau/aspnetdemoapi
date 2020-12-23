using DemoApi.Infrastructure;

namespace DemoApi.Models
{
	// Represents the resource that is presented (note the rate converted from cents to a decimal)
	// Splitting the objects give us flexibility
	public class Room : Resource
	{
		[Sortable]
		[SearchableString]
		public string Name { get; set; }

		[Sortable(Default = true)]
		[SearchableDecimal]
		public decimal Rate { get; set; }

		public Form Book { get; set; }
	}
}