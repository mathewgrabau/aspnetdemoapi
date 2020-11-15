using System;
using System.Threading.Tasks;
using AutoMapper;
using DemoApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoApi.Services
{
	public class DefaultRoomService : IRoomService
	{
		private readonly HotelApiDbContext _context;
		private readonly IMapper _mapper;

		public DefaultRoomService(HotelApiDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		public async Task<Room> GetRoomAsync(Guid id)
		{
			var entity = await _context.Rooms.SingleOrDefaultAsync(x => x.Id == id);

			if (entity == null)
			{
				// SEnds back the 404
				return null;
			}

			// Nicely done mapping here.
			var roomResource = _mapper.Map<Room>(entity);

			return roomResource;
		}
	}
}