using System;
using System.Text;
using System.Threading;
using EspIot.Core.Collections;
using EspIot.Drivers.Wifi.Events;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace EspIot.Drivers.Mqtt
{
    public class MqttClientWrapper
    {
        private const ushort
            MessageQueueSizeLimit = 50; //Limit maximum queued messages to avoid potential OutOfMemory Exception

        private readonly string _brokerAddress;
        private readonly MqttClient _client;
        private readonly string _deviceId;
        private readonly ConcurrentQueue _messageQue = new ConcurrentQueue();
        private readonly string _readTopic;
        private readonly string _sendTopic;
        private Thread _connectionWatcherThread;
        private Thread _outboundMessageSendingThread;
        private bool _status;

        public MqttClientWrapper(string brokerAddress, string deviceId)
        {
            _brokerAddress = brokerAddress;
            _deviceId = deviceId;
            _sendTopic = $"devices/{_deviceId}/messages/events/";
            _client = new MqttClient(_brokerAddress);
            _client.MqttMsgPublishReceived += OnMessagReceived;
            _client.MqttMsgSubscribed += OnSubscribed;
        }

        public event MqttConnectedEventHandler OnMqttClientConnected;
        public event MqttDisconnectedEventHandler OnMqttClientDisconnected;

        public void Connect()
        {
            TryConnect();
            StartConnectionWatcher();
            StartOutboundMessageWorker();
        }

        public void Publish(MqttOutboundMessage message)
        {
            if (_messageQue.Count >= MessageQueueSizeLimit)
            {
                return; //ignore message if queue is full
            }

            _messageQue.Enqueue(message);
        }

        public void Subscribe(string[] topics)
        {
            _client.Subscribe(topics, new byte[] {2});
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

        private void StartOutboundMessageWorker()
        {
            _outboundMessageSendingThread = new Thread(() =>
            {
                while (true)
                {
                    var message = _messageQue.Dequeue() as MqttOutboundMessage;

                    bool isMessageSent = false;

                    for (ushort i = 0; i < 3; i++)
                    {
                        try
                        {
                            if (!_client.IsConnected)
                            {
                                TryConnect();
                            }

                            string topic = string.Format("{0}{1}", _sendTopic, message.Params);
                            Console.WriteLine("Publish on Topic:" + topic + " Message:" + message.Payload);
                            _client.Publish(topic, Encoding.UTF8.GetBytes(message.Payload),
                                MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
                            isMessageSent = true;
                            break;
                        }
                        catch (Exception)
                        {
                            // ignored
                        }
                    }

                    if (!isMessageSent)
                    {
                        throw new Exception(
                            "MqttClient critical exception: Cannot publish message despite of connection with broker.");
                    }
                }
            });

            _outboundMessageSendingThread.Start();
        }

        private void TryConnect()
        {
            while (!_client.IsConnected)
            {
                if (_status)
                {
                    _status = false;
                    OnMqttClientDisconnected();
                }

                try
                {
                    Console.WriteLine("Attempt connect to MQTT broker...");
                    _client.Connect("");
                    Console.WriteLine("Connected successfully to MQTT broker");
                    if (!_status)
                    {
                        _status = true;
                        OnMqttClientConnected();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error during connection to MQTT broker");
                }

                Thread.Sleep(15000);
            }
        }

        private void StartConnectionWatcher()
        {
            if (_connectionWatcherThread != null)
            {
                return;
            }

            _connectionWatcherThread = new Thread(() =>
            {
                while (true)
                {
                    if (!_client.IsConnected)
                    {
                        TryConnect();
                    }

                    Thread.Sleep(15000);
                }
            });
            _connectionWatcherThread.Start();
        }
    }
}