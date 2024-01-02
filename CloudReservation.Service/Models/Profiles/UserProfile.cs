using AutoMapper;
using CloudReservation.DAL.Entities;
using CloudReservation.Service.Commands.UserCommands.Create;
using CloudReservation.Service.Models.UserModels;

namespace CloudReservation.Service.Models.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<CreateUserCommand, Employee>()
            .ForMember(dest => dest.Claims, act => act.Ignore());
        CreateMap<UserClaimsDto, EmployeeClaim>();
        CreateMap<Employee, UserDto>();
    }
}