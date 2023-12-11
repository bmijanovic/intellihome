﻿using IntelliHome_Backend.Features.Shared.Handlers.Interfaces;
using IntelliHome_Backend.Features.Shared.Services.Interfaces;
using IntelliHome_Backend.Features.VEU.Handlers.Interfaces;
using MQTTnet.Client;
using MQTTnet;
using Data.Models.Shared;
using IntelliHome_Backend.Features.Shared.Hubs.Interfaces;
using IntelliHome_Backend.Features.Shared.Hubs;
using Microsoft.AspNetCore.SignalR;
using IntelliHome_Backend.Features.VEU.DTOs;
using IntelliHome_Backend.Features.VEU.Services.Interfaces;
using Newtonsoft.Json;
using IntelliHome_Backend.Features.Home.Handlers;

namespace IntelliHome_Backend.Features.VEU.Handlers
{
    public class SolarPanelSystemHandler : SmartDeviceHandler, ISolarPanelSystemHandler
    {
        public SolarPanelSystemHandler(IMqttService mqttService, IServiceProvider serviceProvider, ISimulationsHandler simualtionsHandler, IHubContext<SmartDeviceHub, ISmartDeviceClient> smartDeviceHubContext)
            : base(mqttService, serviceProvider, simualtionsHandler, smartDeviceHubContext)
        {
            this.mqttService.SubscribeAsync($"FromDevice/+/{SmartDeviceCategory.VEU}/{SmartDeviceType.SOLARPANELSYSTEM}/+", HandleMessageFromDevice);
        }

        protected override async Task HandleMessageFromDevice(MqttApplicationMessageReceivedEventArgs e)
        {
            String[] topic_parts = e.ApplicationMessage.Topic.Split('/');
            if (topic_parts.Length < 5)
            {
                Console.WriteLine("Error handling topic");
                return;
            }
            string solarPanelSystemId = topic_parts.Last();
            _ = smartDeviceHubContext.Clients.Group(solarPanelSystemId).ReceiveSmartDeviceData(e.ApplicationMessage.ConvertPayloadToString());

            using var scope = serviceProvider.CreateScope();
            var solarPanelSystemService = scope.ServiceProvider.GetRequiredService<ISolarPanelSystemService>();
            var solarPanelSystem = await solarPanelSystemService.Get(Guid.Parse(solarPanelSystemId));
            if (solarPanelSystem != null)
            {
                var solarPanelSystemData = JsonConvert.DeserializeObject<SolarPanelSystemProductionDataDTO>(e.ApplicationMessage.ConvertPayloadToString());
                var solarPanelSystemDataInflux = new Dictionary<string, object>
                    {
                        { "production_per_minute", solarPanelSystemData.ProductionPerMinute}
                    };
                var solarPanelSystemDataTags = new Dictionary<string, string>
                    {
                        { "device_id", solarPanelSystem.Id.ToString() }
                    };
                //solarPanelSystemService.AddProductionMeasurement(solarPanelSystemDataInflux, solarPanelSystemDataTags);
            }
        }
    }
}
