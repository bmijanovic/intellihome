﻿using IntelliHome_Backend.Features.Home.Handlers.Interfaces;
using IntelliHome_Backend.Features.Shared.Hubs.Interfaces;
using IntelliHome_Backend.Features.Shared.Hubs;
using IntelliHome_Backend.Features.Shared.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using MQTTnet.Client;
using Data.Models.Home;
using MQTTnet;
using Newtonsoft.Json;
using IntelliHome_Backend.Features.Home.Services.Interfaces;
using IntelliHome_Backend.Features.Home.DTOs;
using IntelliHome_Backend.Features.Shared.Services;

namespace IntelliHome_Backend.Features.Home.Handlers
{
    public class SmartHomeHandler : ISmartHomeHandler
    {
        protected readonly IMqttService mqttService;
        protected readonly IServiceProvider serviceProvider;
        protected readonly IHubContext<SmartHomeHub, ISmartHomeClient> smartHomeHubContext;
        protected readonly IConfiguration configuration;

        public SmartHomeHandler(IConfiguration configuration, MqttFactory mqttFactory, IServiceProvider serviceProvider, IHubContext<SmartHomeHub, ISmartHomeClient> smartHomeHubContext)
        {
            this.serviceProvider = serviceProvider;
            this.smartHomeHubContext = smartHomeHubContext;
            this.configuration = configuration;
            mqttService = new MqttService(mqttFactory);
            mqttService.ConnectAsync(configuration["MqttBroker:Host"], Convert.ToInt32(configuration["MqttBroker:Port"])).Wait();
            mqttService.SubscribeAsync($"FromSmartHome/+/Usage", HandleMessageFromHome);
        }

        public async void SubscribeToSmartHome(SmartHome smartHome)
        {
            string topic = $"FromSmartHome/{smartHome.Id}";
            await mqttService.SubscribeAsync(topic, HandleMessageFromHome);
        }

        public async void PublishMessageToSmartHome(SmartHome smartHome, string payload)
        {
            string topic = $"ToSmartHome/{smartHome.Id}";
            await mqttService.PublishAsync(topic, payload);
        }

        protected async Task HandleMessageFromHome(MqttApplicationMessageReceivedEventArgs e)
        {
            String[] topic_parts = e.ApplicationMessage.Topic.Split('/');
            if (topic_parts.Length < 3)
            {
                Console.WriteLine("Error handling topic");
                return;
            }
            string smartHomeId = topic_parts[1];
            _ = smartHomeHubContext.Clients.Group(smartHomeId).ReceiveSmartHomeData(e.ApplicationMessage.ConvertPayloadToString());

            using var scope = serviceProvider.CreateScope();
            var smartHomeService = scope.ServiceProvider.GetRequiredService<ISmartHomeService>();
            var smartHome = await smartHomeService.Get(Guid.Parse(smartHomeId));
            if (smartHome != null)
            {
                var smartHomeUsageData = JsonConvert.DeserializeObject<SmartHomeUsageDataDTO>(e.ApplicationMessage.ConvertPayloadToString());
                var smartHomeUsageDataInflux = new Dictionary<string, object>
                    {
                        { "productionPerMinute", smartHomeUsageData.ProductionPerMinute },
                        { "consumptionPerMinute", smartHomeUsageData.ConsumptionPerMinute },
                        { "gridPerMinute", smartHomeUsageData.GridPerMinute }
                    };
                var smartHomeUsageDataTags = new Dictionary<string, string>
                    {
                        { "deviceId", smartHome.Id.ToString() }
                    };
                smartHomeService.AddUsageMeasurement(smartHomeUsageDataInflux, smartHomeUsageDataTags);
            }
        }
    }
}
