namespace CloudReservation.Service.Wrappers.ConfigurationWrapper;

public enum ConfigurationType
{
    Key,
    Issuer,
    Audience,
    TimeToLive,
    RoomListEmail,
    TenantId,
    ClientId,
    ClientSecret,
    Rooms
}

public class ConfigurationWrapper : IConfigurationWrapper
{
    private readonly IConfiguration _configuration;

    public ConfigurationWrapper(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public T GetJwtConfiguration<T>(ConfigurationType configurationType)
    {
        return _configuration.GetValue<T>($"Jwt:{configurationType.ToString()}")!;
    }


    public T GetMsGraphConfiguration<T>(ConfigurationType configurationType)
    {
        return typeof(T).IsArray ?
            _configuration.GetSection($"MSGraph:{configurationType.ToString()}").Get<T>()! :
            _configuration.GetValue<T>($"MSGraph:{configurationType.ToString()}")!;
    }
}