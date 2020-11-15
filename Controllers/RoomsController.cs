using System;
using Microsoft.AspNetCore.Mvc;

namespace DemoApi.Controllers
{
	[ApiController]
	[Route("/[controller]")]    // Matches name of the controller without "Controller"
	public class RoomsController : ControllerBase
	{
		[HttpGet(Name = nameof(GetRooms))]
		public IActionResult GetRooms()
		{
			throw new NotImplementedException();
		}
	}
}