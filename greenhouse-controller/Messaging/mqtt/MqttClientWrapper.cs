using System;
using System.Text;
using System.Threading;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace greenhouse_controller.Messaging.azure_iot_hub_mqtt_client
{
    public class MqttClientWrapper
    {
        private readonly string _brokerAddress;
        private readonly string _deviceId;
        private readonly string _sendTopic;
        private readonly string _readTopic;
        private readonly MqttClient _client;

        public MqttClientWrapper(string brokerAddress, string deviceId)
        {
            _brokerAddress = brokerAddress;
            _deviceId = deviceId;
            _sendTopic = string.Format("devices/{0}/messages/events/", _deviceId);
            _client = new MqttClient(_brokerAddress);
            _client.MqttMsgPublishReceived += OnMessagReceived;
            _client.MqttMsgSubscribed += OnSubscribed;
        }

        public void Connect()
        {
            _client.Connect("");
        }

        public void Publish(string message, string properties = "")
        {
            if (!_client.IsConnected)
            {
                TryReconnect();
            }
            _client.Publish(string.Format("{0}{1}", _sendTopic, properties), Encoding.UTF8.GetBytes(message), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
        }

        public void Subscribe(string[] topics)
        {
            _client.Subscribe(topics, new byte[] { 2 });
        }

        private void OnMessagReceived(object sender, MqttMsgPublishEventArgs e)
        {
            string topic = e.Topic;

            string message = Encoding.UTF8.GetString(e.Message, 0, e.Message.Length);

            Console.WriteLine("Publish Received Topic:" + topic + " Message:" + message);
        }

        private void OnSubscribed(object sender, MqttMsgSubscribedEventArgs e)
        {
            Console.WriteLine("Client_MqttMsgSubscribed ");
        }

        private void TryReconnect()
        {
            while (!_client.IsConnected)
            {
                try
                {
                    Console.WriteLine("Attempt reconnect to MQTT broker...");
                    Connect();
                    Console.WriteLine("Recconected successfully to MQTT broker");

                }
                catch (Exception e)
                {
                    Console.WriteLine("Error during reconnection to MQTT broker");
                }
                Thread.Sleep(30000);
            }
        }
    }
}
