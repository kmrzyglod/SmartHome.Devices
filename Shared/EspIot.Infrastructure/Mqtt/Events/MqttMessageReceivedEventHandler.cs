using uPLibrary.Networking.M2Mqtt.Messages;

namespace EspIot.Infrastructure.Mqtt.Events
{
    public delegate void MqttMessageReceivedEventHandler(object sender, MqttMsgPublishEventArgs e);
}
