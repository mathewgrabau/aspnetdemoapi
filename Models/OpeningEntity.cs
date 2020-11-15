using System;

namespace DemoApi.Models
{
	public class OpeningEntity : BookingRange
	{
		public Guid RoomId { get; set; }

		public int Rate { get; set; }
	}
}
