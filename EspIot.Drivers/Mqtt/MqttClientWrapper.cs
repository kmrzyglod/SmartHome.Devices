﻿using EspIot.Core.Collections;
using EspIot.Drivers.Wifi.Events;
using System;
using System.Collections;
using System.Text;
using System.Threading;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace EspIot.Drivers.Mqtt
{
    public class MqttClientWrapper
    {
        private const ushort MESSAGE_QUEUE_SIZE_LIMIT = 50; //Limit maximum queued messages to avoid potential OutOfMemory Exception
        private readonly string _brokerAddress;
        private readonly string _deviceId;
        private readonly string _sendTopic;
        private readonly string _readTopic;
        private readonly MqttClient _client;
        private bool _status = false;
        private Thread _connectionWatcherThread;
        private Thread _outoboundMessageSendingThread;
        private ConcurrentQueue _messageQue = new ConcurrentQueue();
        
        public event MqttConnectedEventHandler OnMqttClientConnected;
        public event MqttDisconnectedEventHandler OnMqttClientDisconnected;

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
            TryConnect();
            StartConnectionWatcher();
            StartOutboundMessageWorker();
        }

        public void Publish(MqttOutboundMessage message)
        {
            if(_messageQue.Count >= MESSAGE_QUEUE_SIZE_LIMIT)
            {
                return; //ignore message if queue is full
            }
            
            _messageQue.Enqueue(message);
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

        private void StartOutboundMessageWorker()
        {

            _outoboundMessageSendingThread = new Thread(new ThreadStart(() =>
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

                            _client.Publish(string.Format("{0}{1}", _sendTopic, message.Params), Encoding.UTF8.GetBytes(message.Payload), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
                            isMessageSent = true;
                            break;
                        }
                        catch (Exception e)
                        {
                            continue;
                        }
                    }

                    if(!isMessageSent)
                    {
                        throw new Exception("MqttClient critical exception: Cannot publish message despite of connection with broker.");
                    }

                }
            }));

            _outoboundMessageSendingThread.Start();
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

            _connectionWatcherThread = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    if (!_client.IsConnected)
                    {
                        TryConnect();
                    }
                    Thread.Sleep(15000);
                }
            }));
            _connectionWatcherThread.Start();
        }


    }
}