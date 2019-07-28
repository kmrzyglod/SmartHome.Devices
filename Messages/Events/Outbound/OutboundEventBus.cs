using EspIot.Core.Messaging.Events;
using EspIot.Core.Messaging.Interfaces;
using EspIot.Drivers.Mqtt;
using Json.NetMF;

namespace Infrastructure.Events.Outbound
{
    public class MqttOutboundEventBus : IOutboundEventBus
    {
        private readonly MqttClientWrapper _mqttClient;
        private const string OUTBOUND_EVENTS_TOPIC = "outbound-events/";

        public MqttOutboundEventBus(MqttClientWrapper mqttClient)
        {
            _mqttClient = mqttClient;
        }
        
        public void Send(IMessage eventMessage)
        {
            _mqttClient.Publish(new MqttOutboundMessage($"{OUTBOUND_EVENTS_TOPIC}{eventMessage.GetType().Name}", JsonSerializer.SerializeObject(eventMessage)));
        }
    }
}
