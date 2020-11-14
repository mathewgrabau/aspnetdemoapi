using Microsoft.AspNetCore.Mvc;

namespace DemoApi.Controllers
{
	[Route("/")]
	[ApiController] // Adds nice features
	public class RootController : ControllerBase    // Stripped down class 
	{
		// Using a named route to be able to resolve the url below
		[HttpGet(Name = nameof(GetRoot))]   // Handle the get path.
		public IActionResult GetRoot()
		{
			// can return status codes, etc.
			var response = new
			{
				href = Url.Link(nameof(GetRoot), null),
				rooms = new
				{
					href = Url.Link(nameof(RoomsController.GetRooms), null)
				}
			};

			return Ok(response);
		}
	}
}