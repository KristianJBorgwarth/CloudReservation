namespace CloudReservation.Service.Models.UserModels;
//TODO: we should probably move claim type and value to another file

public enum ClaimType
{
    Booking
}

public enum ClaimValue
{
    MeetingRoom,
}

/// <summary>
/// This is the data transfer object for creating a user.
/// </summary>
public class UserClaimsDto
{
    public ClaimType ClaimType { get; init; }
    public ClaimValue ClaimValue { get; init; }
}