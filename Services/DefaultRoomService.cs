﻿using System;
using System.Collections.Generic;
using System.Linq;
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

		public DefaultRoomService(
			HotelApiDbContext context,
			IConfigurationProvider mappingConfiguration)
		{
			_context = context;
			_mappingConfiguration = mappingConfiguration;
		}

		public async Task<Room> GetRoomAsync(Guid id)
		{
			var entity = await _context.Rooms
				.SingleOrDefaultAsync(x => x.Id == id);

			if (entity == null)
			{
				return null;
			}

			var mapper = _mappingConfiguration.CreateMapper();
			return mapper.Map<Room>(entity);
		}

		public async Task<PagedResults<Room>> GetRoomsAsync(PagingOptions pagingOptions, 
			SortOptions<Room, RoomEntity> sortOptions,
			SearchOptions<Room, RoomEntity> searchOptions)
		{
			IQueryable<RoomEntity> query = _context.Rooms;
			query = searchOptions.Apply(query);	// Filter before searching
			query = sortOptions.Apply(query); 

			var size = await query.CountAsync();

			var pagedRooms = await query
				.Skip(pagingOptions.Offset.Value).Take(pagingOptions.Limit.Value)
				.ProjectTo<Room>(_mappingConfiguration)
				.ToArrayAsync();

			return new PagedResults<Room> { Items = pagedRooms, TotalSize = size };
		}
	}
}
