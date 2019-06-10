using greenhouse_controller.Core.Messaging;
using System;
using System.Net;
using System.Text;
using uPLibrary.Networking.M2Mqtt;

namespace greenhouse_controller.Messaging.azure_iot_hub_mqtt_client
{
    public class AzureIotMqttClient : IMqttClient
    {
        private readonly string _iotHubUrl;
        private readonly int _gatewayMqttPort;
        private readonly string _deviceId;
        private readonly string _sasToken;
        private readonly string _sendTopic;
        private readonly string _readTopic;
        private readonly MqttClient _client;

        public AzureIotMqttClient(string iotHubUrl, int gatewayMqttPort, string deviceId, string sasToken)
        {
            _iotHubUrl = iotHubUrl;
            _gatewayMqttPort = gatewayMqttPort;
            _deviceId = deviceId;
            _sasToken = sasToken;
            _sendTopic = string.Format("devices/{0}/messages/events", _deviceId);
            var ip = Dns.GetHostEntry(_iotHubUrl).AddressList[0].ToString();
            _client = new MqttClient(ip, _gatewayMqttPort, true, null, null, MqttSslProtocols.TLSv1_1);
        }

        public void Connect()
        {
            string username = string.Format("{0}/{1}/?api-version=2018-06-30", _iotHubUrl, _deviceId);
            string password = _sasToken;
            try
            {
                _client.Connect(_deviceId, username, password);
            }
            catch (Exception e)
            {
                //
            }
        }

        public void Publish(string message)
        {
            _client.Publish(_sendTopic, Encoding.UTF8.GetBytes(message));
        }
    }
}
