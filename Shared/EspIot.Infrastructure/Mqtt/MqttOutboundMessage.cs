namespace EspIot.Infrastructure.Mqtt
{
    public class MqttOutboundMessage
    {
        public MqttOutboundMessage(string @params, string payload)
        {
            Params = @params;
            Payload = payload;
        }

        public string Params { get;}
        public string Payload { get;}
    }
}
