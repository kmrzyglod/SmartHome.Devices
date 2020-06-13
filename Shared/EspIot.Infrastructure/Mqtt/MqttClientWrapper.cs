using System;
using System.Text;
using System.Threading;
using EspIot.Core.Collections;
using EspIot.Core.Helpers;
using EspIot.Drivers.StatusLed;
using EspIot.Infrastructure.Mqtt.Events;
using EspIot.Infrastructure.Wifi;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace EspIot.Infrastructure.Mqtt
{
    public class MqttClientWrapper
    {
        private const ushort
            MessageQueueSizeLimit = 10; //Limit maximum queued messages to avoid potential OutOfMemory Exception

        private readonly string _brokerAddress;
        private readonly MqttClient _client;
        private readonly string _deviceId;
        private readonly ConcurrentQueue _messageQue = new ConcurrentQueue();
        private readonly string _sendTopic;
        private Thread _connectionWatcherThread;
        private Thread _outboundMessageSendingThread;
        private bool _status;
        private string[] _topics;
        private bool _isConnectedToNetwork = false;


        public MqttClientWrapper(string brokerAddress, string deviceId, StatusLed statusLed)
        {
            _brokerAddress = brokerAddress;
            _deviceId = deviceId;
            _sendTopic = $"devices/{_deviceId}/messages/events/";
            _client = new MqttClient(_brokerAddress);
            _client.MqttMsgPublishReceived += OnMessageReceived;
            WifiDriver.OnWifiConnected += () => { _isConnectedToNetwork = true;};
            WifiDriver.OnWifiDisconnected += () => { _isConnectedToNetwork = false;};
            WifiDriver.OnWifiDuringConnection += () => { _isConnectedToNetwork = false;};
        }

        public event MqttConnectedEventHandler OnMqttClientConnected;
        public event MqttDisconnectedEventHandler OnMqttClientDisconnected;
        public event MqttMessageReceivedEventHandler OnMqttMessageReceived;

        public void Connect(string[] topics)
        {
            _topics = topics;
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

        private void Subscribe(string[] topics)
        {
            _client.Subscribe(topics, new byte[] {2});
        }

        private void OnMessageReceived(object sender, MqttMsgPublishEventArgs e)
        {
            string topic = e.Topic;

            Logger.Log(() =>
            {
                string message = Encoding.UTF8.GetString(e.Message, 0, e.Message.Length);
                return $"Publish Received Topic: {topic} Message: {message}";
            });
            
            OnMqttMessageReceived?.Invoke(sender, e);
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

                            string topic = $"{_sendTopic}{message.Params}";
                            Logger.Log(() => $"Publish on Topic: {topic} Message: {message.Payload}");
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
                if (!_isConnectedToNetwork)
                {
                    Thread.Sleep(1000);
                    continue;
                }
                
                if (_status)
                {
                    _status = false;
                    OnMqttClientDisconnected?.Invoke();
                }

                try
                {
                    Logger.Log("Attempt connect to MQTT broker...");
                    _client.Connect("");
                    Logger.Log("Connected successfully to MQTT broker");
                    if (!_status)
                    {
                        _status = true;
                        Subscribe(_topics);
                        OnMqttClientConnected?.Invoke();
                    }
                }
                catch (Exception)
                {
                    Logger.Log("Error during connection to MQTT broker");
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