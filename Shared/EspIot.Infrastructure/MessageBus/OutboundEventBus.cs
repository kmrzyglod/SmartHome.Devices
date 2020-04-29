using EspIot.Application.Interfaces;
using EspIot.Infrastructure.Mqtt;
using Json.NetMF;

namespace EspIot.Infrastructure.MessageBus
{
    public class MqttOutboundEventBus : IOutboundEventBus
    {
        private readonly MqttClientWrapper _mqttClient;

        public MqttOutboundEventBus(MqttClientWrapper mqttClient)
        {
            _mqttClient = mqttClient;
        }
        
        public void Send(IEvent eventMessage)
        {
            _mqttClient.Publish(new MqttOutboundMessage($"MessageType={eventMessage.GetType().Name}", JsonSerializer.SerializeObject(eventMessage)));
        }
    }
}
