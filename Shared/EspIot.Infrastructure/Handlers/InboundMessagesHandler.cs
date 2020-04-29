using System;
using System.Collections;
using System.Text;
using EspIot.Application.Events.Outbound;
using EspIot.Application.Interfaces;
using EspIot.Core.Messaging.Enum;
using EspIot.Core.Messaging.Validation;
using EspIot.Infrastructure.Mqtt;
using Json.NetMF;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace EspIot.Infrastructure.Handlers
{
    public class InboundMessagesHandler
    {
        public InboundMessagesHandler(MqttClientWrapper mqttClient, ICommandBus commandBus,
            ICommandsFactory commandsFactory, IOutboundEventBus outboundEventBus)
        {
            mqttClient.OnMqttMessageReceived += (_, args) =>
            {
                try
                {
                    var decodedMessage = DecodeMqttMessage(args);
                    var command = commandsFactory.Create(decodedMessage.Name, decodedMessage.Payload);
                    var errors = command.Validate();
                    if (errors.Length > 0)
                    {
                        outboundEventBus.Send(new CommandResultEvent(command.CorrelationId, StatusCode.ValidationError,
                            command.GetType().Name, errors.SerializeToString()));
                        return;
                    }

                    commandBus.Send(command);
                }
                catch (Exception e)
                {
                    outboundEventBus.Send(new ErrorEvent(
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