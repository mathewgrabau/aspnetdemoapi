﻿using AutoMapper;
using DemoApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoApi.Services
{
	public class DefaultBookingService : IBookingService
	{
		private readonly HotelApiDbContext _context;
		private readonly IDateLogicService _dateLogicService;
		private readonly IMapper _mapper;

		public DefaultBookingService(
			HotelApiDbContext context,
			IDateLogicService dateLogicService,
			IMapper mapper)
		{
			_context = context;
			_dateLogicService = dateLogicService;
			_mapper = mapper;
		}

		public async Task<Guid> CreateBookingAsync(
			Guid userId,
			Guid roomId,
			DateTimeOffset startAt,
			DateTimeOffset endAt)
		{
			// Lookup the room reference to do it.
			var room = await _context.Rooms.SingleOrDefaultAsync(r => r.Id == roomId);
			// Error out if required.
			if (room == null)
            {
				throw new ArgumentException("Invalid room ID");
            }

			// Calculate how much 
			var minimumStay = _dateLogicService.GetMinimumStay();
			var total = (int)((endAt - startAt).TotalHours / minimumStay.TotalHours) * room.Rate;

			var bookingId = Guid.NewGuid();

			var newBooking = _context.Bookings.Add(new BookingEntity
			{
				Id = bookingId,
				CreatedAt = DateTimeOffset.UtcNow,
				ModifiedAt = DateTimeOffset.UtcNow,
				StartAt = startAt.ToUniversalTime(),
				EndAt = endAt.ToUniversalTime(),
				Total = total,
				Room = room
			});

			var created = await _context.SaveChangesAsync();
			if (created < 1)
            {
				throw new InvalidOperationException("Could not create the booking.");
            }

			return bookingId;
		}

		public async Task<Booking> GetBookingAsync(Guid bookingId)
		{
			var entity = await _context.Bookings
				.SingleOrDefaultAsync(b => b.Id == bookingId);

			if (entity == null) return null;

			return _mapper.Map<Booking>(entity);
		}

		public async Task DeleteBookingAsync(Guid bookingId)
        {
			var booking = await _context.Bookings.SingleOrDefaultAsync(b => b.Id == bookingId);
			if (booking == null)
            {
				return;
            }

			_context.Bookings.Remove(booking);
			await _context.SaveChangesAsync();
        }
	}
}
