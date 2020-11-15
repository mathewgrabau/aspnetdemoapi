namespace DemoApi.Models
{
	// Represents the resource that is presented (note the rate converted from cents to a decimal)
	// Splitting the objects give us flexibility
	public class Room : Resource
	{
		public string Name { get; set; }

		public decimal Rate { get; set; }
	}
}