using AutoMapper;
using DemoApi.Models;

namespace DemoApi.Infrastructure
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			// Replaces the conversion code. 
			// Needs the Href UrlLink as well.
			CreateMap<RoomEntity, Room>().ForMember(dest => dest.Rate, opt => opt.MapFrom(src => src.Rate / 100.0m));
		}
	}
}