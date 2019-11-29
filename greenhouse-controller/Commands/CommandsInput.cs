using System.Collections;
using System.Text;
using EspIot.Core.Messaging.Interfaces;
using EspIot.Drivers.Mqtt;
using Json.NetMF;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace GreenhouseController.Commands
{
    internal class CommandsInput
    {
        private readonly MqttClientWrapper _mqttClient;
        private readonly ICommandBus _commandBus;

        public CommandsInput(MqttClientWrapper mqttClient, ICommandBus commandBus)
        {
            _mqttClient = mqttClient;
            _commandBus = commandBus;
            _mqttClient.OnMqttMessageReceived += (sender, args) =>
            {
                var decodedMessage = DecodeMqttMessage(args);
                var command = CommandsFactory.Create(decodedMessage.Name, decodedMessage.Payload);
                _commandBus.Send(command);
            };
        }

        private MqttMessage DecodeMqttMessage(MqttMsgPublishEventArgs e)
        {
            foreach (string param in e.Topic.Split('&'))
            {
                if (param.IndexOf("command-name") >= 0)
                {
                    return new MqttMessage(param.Split('=')[1],
                        JsonSerializer.DeserializeString(Encoding.UTF8.GetString(e.Message, 0, e.Message.Length)) as
                            Hashtable);
                }
            }

            return null;
        }

        protected class MqttMessage
        {
            public MqttMessage(string name, Hashtable payload)
            {
                Name = name;
                Payload = payload;
            }

            public string Name { get; }
            public Hashtable Payload { get; }
        }
    }
}