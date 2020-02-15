using System;
using System.Collections;
using System.Text;
using EspIot.Core.Messaging.Concrete;
using EspIot.Core.Messaging.Enum;
using EspIot.Core.Messaging.Events;
using EspIot.Core.Messaging.Interfaces;
using EspIot.Infrastructure.Mqtt;
using Json.NetMF;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace EspIot.Infrastructure.Handlers
{
    public class InboundMessagesHandler
    {
        private readonly ICommandBus _commandBus;
        private readonly ICommandsFactory _commandsFactory;
        private readonly MqttClientWrapper _mqttClient;
        private readonly IOutboundEventBus _outboundEventBus;

        public InboundMessagesHandler(MqttClientWrapper mqttClient, ICommandBus commandBus,
            ICommandsFactory commandsFactory, IOutboundEventBus outboundEventBus)
        {
            _mqttClient = mqttClient;
            _commandBus = commandBus;
            _commandsFactory = commandsFactory;
            _outboundEventBus = outboundEventBus;
            _mqttClient.OnMqttMessageReceived += (_, args) =>
            {
                try
                {
                    var decodedMessage = DecodeMqttMessage(args);
                    var command = _commandsFactory.Create(decodedMessage.Name, decodedMessage.Payload);
                    _commandBus.Send(command);
                }
                catch (Exception e)
                {
                    outboundEventBus.Send(new ErrorEvent(Guid.NewGuid().ToString(),
                        $"Exception during handling inbound message: {e.Message}", ErrorLevel.Warning));
                }
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