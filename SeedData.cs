using System;
using System.Linq;
using System.Threading.Tasks;
using DemoApi.Models;
using DemoApi.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DemoApi
{
	public static class SeedData
	{
		public static async Task InitializeAsync(IServiceProvider services)
		{
			await AddTestData(services.GetRequiredService<HotelApiDbContext>(),
				services.GetRequiredService<IDateLogicService>());
		}

		public static async Task AddTestData(HotelApiDbContext context, IDateLogicService dateLogicService)
		{
			// Return if already data (probably not test instance)
			if (context.Rooms.Any())
			{
				return;
			}

			var oxford = new RoomEntity
			{
				Id = Guid.Parse("33ed08b6-cd8c-4fe1-be31-0ac918a9a054"),
				Name = "Oxford Suite",
				Rate = 10119
			};

			context.Rooms.Add(oxford);

			context.Rooms.Add(new RoomEntity
			{
				Id = Guid.Parse("ce9616dc-55c8-48af-b336-332733350889"),
				Name = "Driscoll Suite",
				Rate = 23959
			});

			var today = DateTimeOffset.Now;
			var start = dateLogicService.AlignStartTime(today);
			var end = start.Add(dateLogicService.GetMinimumStay());

			context.Bookings.Add(new BookingEntity
			{
				Id = Guid.Parse("2eac8dea-2749-42b3-9d21-8eb2fc0fd6bd"),
				Room = oxford,
				CreatedAt = DateTimeOffset.UtcNow,
				StartAt = start,
				EndAt = end,
				Total = oxford.Rate,
			});

			await context.SaveChangesAsync();
		}
	}
}