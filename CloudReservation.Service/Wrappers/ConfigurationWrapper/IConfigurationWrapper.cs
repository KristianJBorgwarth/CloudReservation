namespace CloudReservation.Service.Wrappers.ConfigurationWrapper;

public interface IConfigurationWrapper
{
    public T GetJwtConfiguration<T>(ConfigurationType configurationType);
    public T GetMsGraphConfiguration<T>(ConfigurationType configurationType);
}
