using System;
using System.Threading.Tasks;
using DemoApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoApi.Services
{
	public class DefaultRoomService : IRoomService
	{
		private readonly HotelApiDbContext _context;

		public DefaultRoomService(HotelApiDbContext context)
		{
			_context = context;
		}

		public async Task<Room> GetRoomAsync(Guid id)
		{
			var entity = await _context.Rooms.SingleOrDefaultAsync(x => x.Id == id);

			if (entity == null)
			{
				// SEnds back the 404
				return null;
			}

			var roomResource = new Room
			{
				// TODO work around this.
				Href = null, //Url.Link(nameof(GetRoomById), new { roomId = entity.Id }),
				Name = entity.Name,
				Rate = entity.Rate / 100.0m
			};

			return roomResource;
		}
	}
}