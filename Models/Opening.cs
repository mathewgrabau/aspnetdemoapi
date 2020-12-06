using DemoApi.Infrastructure;
using System;

namespace DemoApi.Models
{
	public class Opening
	{
		[Sortable(EntityProperty = nameof(OpeningEntity.RoomId))]
		public Link Room { get; set; }

		[Sortable(Default = true)]
		public DateTimeOffset StartAt { get; set; }

		[Sortable()]
		public DateTimeOffset EndAt { get; set; }

		[Sortable()]
		public decimal Rate { get; set; }
	}
}
