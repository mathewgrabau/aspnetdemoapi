using System;
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
		public ActionResult<HotelInfo> GetInfo()
		{
			_hotelInfo.Self = Link.To(nameof(GetInfo));

			return _hotelInfo;
		}
	}
}
