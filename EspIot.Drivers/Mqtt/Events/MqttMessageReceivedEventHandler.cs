using uPLibrary.Networking.M2Mqtt.Messages;

namespace EspIot.Drivers.Mqtt.Events
{
    public delegate void MqttMessageReceivedEventHandler(object sender, MqttMsgPublishEventArgs e);
}
