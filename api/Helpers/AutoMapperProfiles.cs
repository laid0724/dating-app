using System.Linq;
using AutoMapper;
using DatingApp.API.Dtos;
using DatingApp.API.Models;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>()
                // ? map photo url
                .ForMember(
                  destination => destination.PhotoUrl,
                  options => options
                    .MapFrom(src => src.Photos
                      .FirstOrDefault(photo => photo.IsMain).Url
                    )
                )
                // ? calculate age from DOB
                .ForMember(
                  destination => destination.Age,
                  options => options
                    .MapFrom(src =>
                      src.DateOfBirth.CalculateAge()
                    )
                );
            CreateMap<User, UserForDetailedDto>()
                // ? map photo url
                .ForMember(
                  destination => destination.PhotoUrl,
                  options => options
                    .MapFrom(src => src.Photos
                      .FirstOrDefault(photo => photo.IsMain).Url
                    )
                )
                // ? calculate age from DOB
                .ForMember(
                  destination => destination.Age,
                  options => options
                    .MapFrom(src =>
                      src.DateOfBirth.CalculateAge()
                    )
                );
            CreateMap<Photo, PhotosForDetailedDto>();
        }
    }
}