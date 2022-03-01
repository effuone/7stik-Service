using AutoMapper;
using Zhetistik.Data.AuthModels;
using Zhetistik.Data.Models;

namespace Zhetistik.Data.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserModel, User>()
                .ForMember(dst => dst.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dst => dst.Password, opt => opt.MapFrom(src => src.Password))
                .ForMember(dst => dst.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dst => dst.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dst => dst.UserId, opt => opt.Ignore())
                ;
            
            CreateMap<User, AuthenticateResponse>()
                .ForMember(dst => dst.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dst => dst.FirstName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dst => dst.LastName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dst => dst.Token, opt => opt.Ignore())
                ;
        }
    }
}