using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DemoApi.Models;

namespace DemoApi.Services
{
	public interface IRoomService
	{
		Task<Room> GetRoomAsync(Guid Id);
		Task<PagedResults<Room>> GetRoomsAsync(PagingOptions pagingOptions, 
			SortOptions<Room, RoomEntity> sortOptions);
	}
}
