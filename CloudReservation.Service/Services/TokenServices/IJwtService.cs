using CloudReservation.DAL.Entities;

namespace CloudReservation.Service.Services.TokenServices;

public interface IJwtService
{
    public string GenerateToken(Employee employee);
}