using System;
using System.Threading.Tasks;
using DemoApi.Models;

namespace DemoApi.Services
{
	public interface IRoomService
	{
		Task<Room> GetRoomAsync(Guid Id);
	}
}