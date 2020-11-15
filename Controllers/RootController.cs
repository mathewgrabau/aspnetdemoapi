using Microsoft.AspNetCore.Mvc;

namespace DemoApi.Controllers
{
	[Route("/")]
	[ApiController] // Adds nice features
	[ApiVersion("1.0")] // Specifying the version the controller answers to.
	public class RootController : ControllerBase    // Stripped down class 
	{
		// Using a named route to be able to resolve the url below
		[HttpGet(Name = nameof(GetRoot))]   // Handle the get path.
		[ProducesResponseType(200)] // Helps guide generation of OpenAPI specification (if you are adding that your project)
		public IActionResult GetRoot()
		{
			// can return status codes, etc.
			var response = new
			{
				href = Url.Link(nameof(GetRoot), null),
				rooms = new
				{
					href = Url.Link(nameof(RoomsController.GetRooms), null)
				},
				info = new
				{
					href = Url.Link(nameof(InfoController.GetInfo), null)
				}
			};

			return Ok(response);
		}
	}
}