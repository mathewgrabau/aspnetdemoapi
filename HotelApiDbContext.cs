using DemoApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoApi
{
	public class HotelApiDbContext : DbContext
	{
		public HotelApiDbContext(DbContextOptions options) : base(options)
		{

		}

		// Table objects
		public DbSet<RoomEntity> Rooms { get; set; }

		public DbSet<BookingEntity> Bookings { get; set; }
	}
}