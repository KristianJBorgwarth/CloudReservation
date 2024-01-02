using AutoMapper;
using CloudReservation.Service.Models.RoomModels;
using Microsoft.Graph.Models;

namespace CloudReservation.Service.Models.Profiles
{
    public class RoomProfile : Profile
    {
        public RoomProfile() 
        {
            CreateMap<Room, RoomDto>();
        }
    }
}
