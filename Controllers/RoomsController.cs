using DemoApi.Models;
using DemoApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoApi.Controllers
{
	[Route("/[controller]")]
	[ApiController]
	public class RoomsController : ControllerBase
	{
		private readonly IRoomService _roomService;
		private readonly IOpeningService _openingService;
		private readonly IDateLogicService _dateLogicService;
		private readonly IBookingService _bookingService;
		private PagingOptions _defaultPagingOptions;

		public RoomsController(
			IRoomService roomService,
			IOpeningService openingService,
			IDateLogicService dateLogicService,
			IBookingService bookingService,
			IOptions<PagingOptions> defaultPagingOptionsWrapper)
		{
			_roomService = roomService;
			_openingService = openingService;
			_dateLogicService = dateLogicService;
			_bookingService = bookingService;
			_defaultPagingOptions = defaultPagingOptionsWrapper.Value;
		}

		// GET /rooms
		[HttpGet(Name = nameof(GetAllRooms))]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		public async Task<ActionResult<Collection<Room>>> GetAllRooms(
			[FromQuery] PagingOptions pagingOptions,
			[FromQuery] SortOptions<Room, RoomEntity> sortOptions,
			[FromQuery] SearchOptions<Room, RoomEntity> searchOptions)
		{
			pagingOptions.Offset = pagingOptions.Offset ?? _defaultPagingOptions.Offset;
			pagingOptions.Limit = pagingOptions.Limit ?? _defaultPagingOptions.Limit;

			var rooms = await _roomService.GetRoomsAsync(pagingOptions, sortOptions, searchOptions);

			var collection = PagedCollection<Room>.Create<RoomsResponse>(
				Link.ToCollection(nameof(GetAllRooms)),
				rooms.Items.ToArray(),
				rooms.TotalSize,
				pagingOptions);

			collection.Openings = Link.ToCollection(nameof(GetAllRoomOpenings));

			return collection;
		}

		// GET /rooms/openings
		[HttpGet("openings", Name = nameof(GetAllRoomOpenings))]
		[ProducesResponseType(400)]
		[ProducesResponseType(200)]
		public async Task<ActionResult<Collection<Opening>>> GetAllRoomOpenings(
			[FromQuery] PagingOptions pagingOptions,
			[FromQuery] SortOptions<Opening, OpeningEntity> sortOptions,
			[FromQuery] SearchOptions<Opening, OpeningEntity> searchOptions)
		{
			pagingOptions.Offset = pagingOptions.Offset ?? _defaultPagingOptions.Offset;
			pagingOptions.Limit = pagingOptions.Limit ?? _defaultPagingOptions.Limit;
			
			var openings = await _openingService.GetOpeningsAsync(pagingOptions, sortOptions, searchOptions);

			var collection = PagedCollection<Opening>.Create(Link.ToCollection(nameof(GetAllRoomOpenings)), openings.Items.ToArray(), openings.TotalSize, pagingOptions);

			return collection;
		}

		// GET /rooms/{roomId}
		[HttpGet("{roomId}", Name = nameof(GetRoomById))]
		[ProducesResponseType(404)]
		[ProducesResponseType(200)]
		public async Task<ActionResult<Room>> GetRoomById(Guid roomId)
		{
			var room = await _roomService.GetRoomAsync(roomId);
			if (room == null) return NotFound();

			return room;
		}

		// POST /rooms/{roomId}/bookings
		[HttpPost("{roomId}/bookings", Name = nameof(CreateBookingForRoom))]
		[ProducesResponseType(404)]
		[ProducesResponseType(400)]
		[ProducesResponseType(201)]
		public async Task<ActionResult> CreateBookingForRoom(Guid roomId, [FromBody] BookingForm bookingForm)
		{
			// Assume parameters have been validated (by framework)
			var room = await _roomService.GetRoomAsync(roomId);
			if (room == null)
			{
				return NotFound();
			}

			// Not less than the allowed time for the bookings.
			var minimumStay = _dateLogicService.GetMinimumStay();
			bool tooShort = (bookingForm.EndAt.Value - bookingForm.StartAt.Value) < minimumStay;
			if (tooShort)
			{
				return BadRequest(new ApiError($"Minimum booking duration is {minimumStay.TotalHours} hours"));
			}

			// Need list of conflicting slots
			var conflictingSlots = await _openingService.GetConflictingSlots(roomId, bookingForm.StartAt.Value, bookingForm.EndAt.Value);
			if (conflictingSlots.Any())
			{
				return BadRequest(new ApiError("Time conflicts with an existing booking."));
			}

			// TODO current user (authentication needed)
			// Need a user id, future problem to solve
			var userId = Guid.NewGuid();

			var bookingId = await _bookingService.CreateBookingAsync(userId, roomId, bookingForm.StartAt.Value, bookingForm.EndAt.Value);

			// REturn result with a link that allows navigation to the new booking
			return Created(Url.Link(nameof(BookingsController.GetBookingById), new { bookingId }), null);
		}
	}
}
