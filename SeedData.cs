using System;
using System.Linq;
using System.Threading.Tasks;
using DemoApi.Models;
using Microsoft.Extensions.DependencyInjection;

namespace DemoApi
{
	public static class SeedData
	{
		public static async Task InitializeAsync(IServiceProvider services)
		{
			await AddTestData(services.GetRequiredService<HotelApiDbContext>());
		}

		public static async Task AddTestData(HotelApiDbContext context)
		{
			// Return if already data (probably not test instance)
			if (context.Rooms.Any())
			{
				return;
			}

			context.Rooms.Add(new RoomEntity
			{
				Id = Guid.Parse("33ed08b6-cd8c-4fe1-be31-0ac918a9a054"),
				Name = "Oxford Suite",
				Rate = 10119
			});

			context.Rooms.Add(new RoomEntity
			{
				Id = Guid.Parse("ce9616dc-55c8-48af-b336-332733350889"),
				Name = "Driscoll Suite",
				Rate = 23959
			});

			await context.SaveChangesAsync();
		}
	}
}