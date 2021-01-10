using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Primitives;
using AutoMapper;
using DemoApi;
using DemoApi.Filters;
using DemoApi.Infrastructure;
using DemoApi.Models;
using DemoApi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
			services.TryAddSingleton<ISystemClock, SystemClock>();	// This wasn't working for some reason? AddAuthentication call?

			// Loading some stuff
			// Doing it this was prepares it into DI for loading and injecting.
			services.Configure<HotelInfo>(Configuration.GetSection("Info"));
			services.Configure<HotelOptions>(Configuration);
			services.Configure<PagingOptions>(Configuration.GetSection("DefaultPagingOptions"));

			// Ensure each request receives a new instance.
			services.AddScoped<IRoomService, DefaultRoomService>();
			services.AddScoped<IOpeningService, DefaultOpeningService>();
			services.AddScoped<IBookingService, DefaultBookingService>();
			services.AddScoped<IDateLogicService, DefaultDateLogicService>();
			services.AddScoped<IUserService, DefaultUserService>();

			// Use an in-memory database for development/quick testing
			services.AddDbContext<HotelApiDbContext>(options =>
			{
				options.UseInMemoryDatabase("landondb");
				options.UseOpenIddict<Guid>();
			});

			services.AddOpenIddict()
				.AddCore(options =>
				{
					options.UseEntityFrameworkCore()
						.UseDbContext<HotelApiDbContext>()
						.ReplaceDefaultEntities<Guid>();
				})
				.AddServer(options =>
				{
					options.AddDevelopmentEncryptionCertificate().AddDevelopmentSigningCertificate();
					//options.EnableTokenEndpoint("/token");
					options.SetTokenEndpointUris("/token");
					options.AllowPasswordFlow();
					options.AcceptAnonymousClients();

					options.UseAspNetCore().EnableTokenEndpointPassthrough();
				})
				.AddValidation(options=>
				{
					// TODO try to figure out what to do with this.
					options.UseLocalServer();
					options.UseAspNetCore();
				});

			// ASP.NET Core Identity should use the same claim names as OpenIddict
			services.Configure<IdentityOptions>(options =>
			{
				options.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Name;
				options.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Subject;
				options.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;
			});

			services.AddAuthentication(options =>
			{
				options.DefaultScheme = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
			});

			// Identity
			// services.AddIdentity()	
			// The above is good for web apps because it does cookie.
			AddIdentityCoreServices(services);

			services.AddMvc(options =>
			{
				// Configure static content cache profile definition
				options.CacheProfiles.Add("Static", new CacheProfile { Duration = 86400 });

				options.Filters.Add<JsonExceptionFilter>();
				options.Filters.Add<RequireHttpsOrCloseAttribute>();
				options.Filters.Add<LinkRewritingFilter>();
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

			services.AddAutoMapper(options => options.AddProfile<MappingProfile>());

			services.Configure<ApiBehaviorOptions>(options =>
			{
				options.InvalidModelStateResponseFactory = context =>
				{
					var errorResponse = new ApiError(context.ModelState);
					return new BadRequestObjectResult(errorResponse);
				};
			});

			services.AddResponseCaching();

			services.AddAuthorization(options =>
			{
				options.AddPolicy("ViewAllUsersPolicy", p => p.RequireAuthenticatedUser().RequireRole("Admin"));
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
			app.UseAuthentication();

			app.UseResponseCaching();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}

		private static void AddIdentityCoreServices(IServiceCollection services)
        {
			var builder = services.AddIdentityCore<UserEntity>();
			builder = new IdentityBuilder(builder.UserType, typeof(UserRoleEntity), builder.Services);

			builder.AddRoles<UserRoleEntity>()
				.AddEntityFrameworkStores<HotelApiDbContext>()
				.AddDefaultTokenProviders()
				.AddSignInManager<SignInManager<UserEntity>>();
        }
	}
}
