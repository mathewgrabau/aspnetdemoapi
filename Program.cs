using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DemoApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DemoApi
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var host = CreateHostBuilder(args).Build();
			InitializeDatabase(host);
			host.Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});

		private static void InitializeDatabase(IHost host)
		{
			using (var scope = host.Services.CreateScope())
			{
				var services = scope.ServiceProvider;

				try
				{
					// This is the best practice that currently exists for seeding the database.
					SeedData.InitializeAsync(services).Wait();
				}
				catch (Exception e)
				{
					var logger = services.GetRequiredService<ILogger<Program>>();
					logger.LogError(e, "An error occurred seeding the database");
				}
			}
		}
	}
}
