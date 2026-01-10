using AutoMapper;
using Domain.Entities;
using Google.Apis.Auth;
using System.Security.Claims;
using System.Collections.Generic;

namespace Infrastructure.Mappings
{
    public class InfrastructureProfile : Profile
    {
        public InfrastructureProfile()
        {
            // Google Payload -> User Entity
            CreateMap<GoogleJsonWebSignature.Payload, User>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => 3))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => Guid.NewGuid().ToString()));

            // User Entity -> Claims for JWT
            CreateMap<User, IEnumerable<Claim>>().ConvertUsing(src => new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, src.Id.ToString()),
                new Claim(ClaimTypes.Email, src.Email),
                new Claim(ClaimTypes.Name, src.FullName),
                new Claim(ClaimTypes.Role, src.RoleId.ToString())
            });
        }
    }
}
