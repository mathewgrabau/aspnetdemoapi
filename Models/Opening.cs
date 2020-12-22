using DemoApi.Infrastructure;
using System;

namespace DemoApi.Models
{
	public class Opening
	{
		[Sortable(EntityProperty = nameof(OpeningEntity.RoomId))]
		public Link Room { get; set; }

		[Sortable(Default = true)]
		[SearchableDateTime]
		public DateTimeOffset StartAt { get; set; }

		[Sortable()]
		[SearchableDateTime]
		public DateTimeOffset EndAt { get; set; }

		[Sortable()]
		[SearchableDecimal]
		public decimal Rate { get; set; }
	}
}
