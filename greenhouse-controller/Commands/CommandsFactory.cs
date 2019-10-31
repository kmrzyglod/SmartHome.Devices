using System;
using System.Text;
using EspIot.Drivers.Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace GreenhouseController.Commands
{
    internal class CommandsFactory
    {
        private readonly MqttClientWrapper _mqttClient;

        public CommandsFactory(MqttClientWrapper mqttClient)
        {
            _mqttClient = mqttClient;
            _mqttClient.OnMqttMessageReceived += (sender, args) =>
            {
                var decodedMessage = DecodeMqttMessage(args);
                
;            };
        }

        private MqttMessage DecodeMqttMessage(MqttMsgPublishEventArgs e)
        {
            foreach (string param in e.Topic.Split('&'))
            {
                if (param.IndexOf("command-name") >= 0)
                {
                    return new MqttMessage(param.Split('=')[1],
                        Encoding.UTF8.GetString(e.Message, 0, e.Message.Length));
                }
            }

            return null;
        }

        private class MqttMessage
        {
            private string Name { get; }
            private string Payload { get; }

            public MqttMessage(string name, string payload)
            {
                Name = name;
                Payload = payload;
            }
        }
    }
}