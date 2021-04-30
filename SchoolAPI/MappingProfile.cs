using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Models;

namespace SchoolAPI
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Organization, OrganizationDto>()
                .ForMember(c => c.FullAddress,
                    opt => opt.MapFrom(x => string.Join(", ", x.City, x.Country)));
            
            CreateMap<OrganizationForCreationDto, Organization>();
            CreateMap<OrganizationForUpdateDto, Organization>();

            CreateMap<User, UserDto>()
                .ForMember(c => c.UserName,
                    opt => opt.MapFrom(x => x.UserName));
            CreateMap<UserForCreationDto, User>();
            CreateMap<UserForUpdateDto, User>();


        }
    }
}