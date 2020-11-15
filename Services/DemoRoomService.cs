using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DemoApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoApi.Services
{
	public class DefaultRoomService : IRoomService
	{
		private readonly HotelApiDbContext _context;
		private readonly IConfigurationProvider _mappingConfiguration;

		public DefaultRoomService(HotelApiDbContext context, IConfigurationProvider mappingConfiguration)
		{
			_context = context;
			_mappingConfiguration = mappingConfiguration;
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
			var mapper = _mappingConfiguration.CreateMapper();
			var roomResource = mapper.Map<Room>(entity);

			return roomResource;
		}

		public async Task<IEnumerable<Room>> GetRoomsAsync()
		{
			var query = _context.Rooms.ProjectTo<Room>(_mappingConfiguration);

			return await query.ToArrayAsync();
		}
	}
}