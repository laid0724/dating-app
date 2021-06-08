using API.DTOs;
using API.Entities;
using AutoMapper;

namespace API.Helpers
{
    // configures the mapping of one object to another
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // Add all mappings here!
            CreateMap<AppUser, MemberDto>();
            CreateMap<Photo, PhotoDto>();
        }
    }
}