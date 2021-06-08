using System.Linq;
using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers
{
    // configures the mapping of one object to another
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // Add all mappings here!
            CreateMap<AppUser, MemberDto>()
                // configure custom mapping for its properties via ForMember() method
                .ForMember(
                    dest => dest.PhotoUrl,
                    opt => opt.MapFrom(
                        src => src.Photos.FirstOrDefault(e => e.IsMain).Url
                    )
                )
                .ForMember(
                    dest => dest.Age,
                    opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge())
                );
            CreateMap<Photo, PhotoDto>();
        }
    }
}