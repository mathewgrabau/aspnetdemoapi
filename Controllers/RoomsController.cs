using System;
using System.Threading.Tasks;
using DemoApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DemoApi.Controllers
{
	[ApiController]
	[Route("/[controller]")]    // Matches name of the controller without "Controller"
	public class RoomsController : ControllerBase
	{
		private readonly HotelApiDbContext _context;

		public RoomsController(HotelApiDbContext context)
		{
			_context = context;
		}

		[HttpGet(Name = nameof(GetRooms))]
		public IActionResult GetRooms()
		{
			throw new NotImplementedException();
		}

		// GET /rooms/{roomId}
		[HttpGet("{roomId}", Name = nameof(GetRoomById))]
		[ProducesResponseType(404)]
		public async Task<ActionResult<Room>> GetRoomById(Guid roomId)
		{
			var entity = await _context.Rooms.SingleOrDefaultAsync(x => x.Id == roomId);

			if (entity == null)
			{
				// SEnds back the 404
				return NotFound();
			}

			var roomResource = new Room
			{
				Href = Url.Link(nameof(GetRoomById), new { roomId = entity.Id }),
				Name = entity.Name,
				Rate = entity.Rate / 100.0m
			};

			return roomResource;
		}
	}
}