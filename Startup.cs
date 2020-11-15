using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DemoApi;
using DemoApi.Filters;
using DemoApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSwag.AspNetCore;

namespace DemoApi
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			// Loading some stuff
			// Doing it this was prepares it into DI for loading and injecting.
			services.Configure<HotelInfo>(Configuration.GetSection("Info"));

			// Use an in-memory database for development/quick testing
			services.AddDbContext<HotelApiDbContext>(
				options => options.UseInMemoryDatabase("landondb")
			);

			services.AddMvc(options =>
			{
				options.Filters.Add<JsonExceptionFilter>();
				options.Filters.Add<RequireHttpsOrCloseAttribute>();
			});
			services.AddControllers();
			services.AddOpenApiDocument();
			services.AddApiVersioning(options =>
			{
				options.DefaultApiVersion = new ApiVersion(1, 0);
				// Add a reader for the API versioning to come from header, specified in the media type
				options.ApiVersionReader = new MediaTypeApiVersionReader();
				// Allow it unspecified, and what the default is set to 
				options.AssumeDefaultVersionWhenUnspecified = true;
				// Versions sent in the responses
				options.ReportApiVersions = true;
				options.ApiVersionSelector = new CurrentImplementationApiVersionSelector(options);
			});

#if false
			// This is the example of how to do this.
            // Also the AllowAnyOrigin can be used for development purposes.
			services.AddCors(options =>
			{
				options.AddPolicy("AllowMyApp", policy => policy.WithOrigins("https://example.com"))
			});
#endif
		}
		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseOpenApi();
				app.UseSwaggerUi3();
			}
			else
			{
				app.UseHsts();
			}


#if false
            // Must be high enough
			app.UseCors();
#endif
			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});


		}
	}
}
