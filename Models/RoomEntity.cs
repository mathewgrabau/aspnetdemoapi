using System;

namespace DemoApi.Models
{
	public class RoomEntity
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		// Store as cents, decimals can be tricky to store!
		public int Rate { get; set; }
	}
}