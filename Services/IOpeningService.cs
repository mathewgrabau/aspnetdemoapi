using DemoApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoApi.Services
{
	public interface IOpeningService
	{
		Task<IEnumerable<Opening>> GetOpeningsAsync();

		Task<IEnumerable<BookingRange>> GetConflictingSlots(
			Guid roomId,
			DateTimeOffset start,
			DateTimeOffset end);
	}
}
