using System;
using System.Linq;
using System.Threading.Tasks;
using DemoApi.Models;
using DemoApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DemoApi.Controllers
{
	[ApiController]
	[Route("/[controller]")]    // Matches name of the controller without "Controller"
	public class RoomsController : ControllerBase
	{
		private readonly IRoomService _service;

		public RoomsController(IRoomService service)
		{
			_service = service;
		}

		[HttpGet(Name = nameof(GetAllRooms))]
		[ProducesResponseType(200)]
		public async Task<ActionResult<Collection<Room>>> GetAllRooms()
		{
			var rooms = await _service.GetRoomsAsync();
			var collection = new Collection<Room>
			{
				Self = Link.ToCollection(nameof(GetAllRooms)),
				Value = rooms.ToArray()
			};

			return collection;
		}

		// GET /rooms/{roomId}
		[HttpGet("{roomId}", Name = nameof(GetRoomById))]
		[ProducesResponseType(404)]
		[ProducesResponseType(200)]
		public async Task<ActionResult<Room>> GetRoomById(Guid roomId)
		{
			var room = await _service.GetRoomAsync(roomId);

			if (room == null)
			{
				return NotFound();
			}

			return room;
		}
	}
}