using EspIot.Core.Messaging.Events;
using EspIot.Core.Messaging.Interfaces;
using EspIot.Drivers.Mqtt;
using Json.NetMF;

namespace Infrastructure.Events.Outbound
{
    public class MqttOutboundEventBus : IOutboundEventBus
    {
        private readonly MqttClientWrapper _mqttClient;

        public MqttOutboundEventBus(MqttClientWrapper mqttClient)
        {
            _mqttClient = mqttClient;
        }
        
        public void Send(IMessage eventMessage)
        {
            _mqttClient.Publish(new MqttOutboundMessage($"MessageType={eventMessage.GetType().Name}", JsonSerializer.SerializeObject(eventMessage)));
        }
    }
}
