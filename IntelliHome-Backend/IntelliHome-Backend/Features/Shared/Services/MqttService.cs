﻿using MQTTnet.Client;
using MQTTnet;
using MQTTnet.Packets;
using IntelliHome_Backend.Features.Shared.Services.Interfaces;

namespace IntelliHome_Backend.Features.Shared.Services
{
    public class MqttService : IMqttService
    {
        private readonly IMqttClient _mqttClient;
        private Dictionary<string, Func<MqttApplicationMessageReceivedEventArgs, Task>> _topicHandlers;

        public MqttService(MqttFactory mqttFactory)
        {
            _mqttClient = mqttFactory.CreateMqttClient();
            _topicHandlers = new Dictionary<string, Func<MqttApplicationMessageReceivedEventArgs, Task>>();
        }

        public async Task ConnectAsync(string host, int port, CancellationToken cancellationToken = default)
        {
            if (_mqttClient.IsConnected)
            {
                throw new InvalidOperationException("MqttService is already initialized.");
            }

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(host, port)
                .Build();

            await _mqttClient.ConnectAsync(options, cancellationToken);

            _mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                try
                {
                    String topic = e.ApplicationMessage.Topic;
                    String[] topicParts = topic.Split("/");
                    if (topicParts[0] == "FromDevice")
                    {
                        topicParts[1] = "+";
                        topicParts[4] = "+";
                        topic = string.Join("/", topicParts);
                    }
                    else if (topicParts[0] == "FromSmartHome" && topicParts[2] == "Usage") {
                        topicParts[1] = "+";
                        topic = string.Join("/", topicParts);
                    }
                    if (_topicHandlers.TryGetValue(topic, out var handler))
                    {
                        await handler(e);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            };
        }

        public async Task PublishAsync(string topic, string payload)
        {
            if (_mqttClient.IsConnected)
            {
                var message = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(payload)
                    .Build();

                await _mqttClient.PublishAsync(message);
            }
            else
            {
                // Handle not connected scenario
            }
        }

        public async Task SubscribeAsync(string topic, Func<MqttApplicationMessageReceivedEventArgs, Task> messageHandler)
        {
            await _mqttClient.SubscribeAsync(new MqttTopicFilter { Topic = topic });
            _topicHandlers.Add(topic, messageHandler);
        }

        public async Task UnsubscribeAsync(string topic)
        {
            await _mqttClient.UnsubscribeAsync(topic);
            _topicHandlers.Remove(topic);
        }
    }

}
