using DemoApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace DemoApi
{
	public class HotelApiDbContext : IdentityDbContext<UserEntity, UserRoleEntity, Guid>	// deriving from this takes care of the DbSets that are needed.
	{
		public HotelApiDbContext(DbContextOptions options) : base(options)
		{

		}

		// Table objects
		public DbSet<RoomEntity> Rooms { get; set; }

		public DbSet<BookingEntity> Bookings { get; set; }
	}
}