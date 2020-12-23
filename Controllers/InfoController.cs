using DemoApi.Infrastructure;
using DemoApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DemoApi.Controllers
{
    [Route("/[controller]")]
	[ApiController]
	public class InfoController : ControllerBase
	{
		readonly HotelInfo _hotelInfo;

		public InfoController(IOptions<HotelInfo> hotelInfoWrapper)
		{
			_hotelInfo = hotelInfoWrapper.Value;
		}

		[HttpGet(Name = nameof(GetInfo))]
		[ProducesResponseType(200)]
		[ProducesResponseType(304)]
		[ResponseCache(CacheProfileName = "Static")]
		[Etag]
		public ActionResult<HotelInfo> GetInfo()
		{
			_hotelInfo.Href = Url.Link(nameof(GetInfo), null);

			if (!Request.GetEtagHandler().NoneMatch(_hotelInfo))
            {
				return StatusCode(304, _hotelInfo);
            }

			return _hotelInfo;
		}
	}
}
