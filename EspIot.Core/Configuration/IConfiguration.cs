namespace EspIot.Core.Configuration
{
    public interface IConfiguration
    {
        string MqttBrokerAddress { get; }
        string DeviceId { get; }
    }
}
