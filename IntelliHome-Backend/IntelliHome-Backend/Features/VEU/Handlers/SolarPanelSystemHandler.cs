﻿using IntelliHome_Backend.Features.Shared.Handlers.Interfaces;
using IntelliHome_Backend.Features.Shared.Services.Interfaces;
using IntelliHome_Backend.Features.VEU.Handlers.Interfaces;
using MQTTnet.Client;
using MQTTnet;
using Data.Models.Shared;
using IntelliHome_Backend.Features.Shared.Hubs.Interfaces;
using IntelliHome_Backend.Features.Shared.Hubs;
using Microsoft.AspNetCore.SignalR;
using IntelliHome_Backend.Features.VEU.Services.Interfaces;
using Newtonsoft.Json;
using IntelliHome_Backend.Features.Home.Handlers;
using Data.Models.VEU;
using IntelliHome_Backend.Features.VEU.DTOs.SolarPanelSystem;

namespace IntelliHome_Backend.Features.VEU.Handlers
{
    public class SolarPanelSystemHandler : SmartDeviceHandler, ISolarPanelSystemHandler
    {
        public SolarPanelSystemHandler(IConfiguration configuration, MqttFactory mqttFactory, IServiceProvider serviceProvider, ISimulationsHandler simualtionsHandler, IHubContext<SmartDeviceHub, ISmartDeviceClient> smartDeviceHubContext)
            : base(configuration,mqttFactory, serviceProvider, simualtionsHandler, smartDeviceHubContext)
        {
            mqttService.SubscribeAsync($"FromDevice/+/{SmartDeviceCategory.VEU}/{SmartDeviceType.SOLARPANELSYSTEM}/+", HandleMessageFromDevice);
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
            var solarPanelSystemData = JsonConvert.DeserializeObject<SolarPanelSystemProductionDataDTO>(e.ApplicationMessage.ConvertPayloadToString());
            if (solarPanelSystem != null && solarPanelSystemData != null)
            {
                var fields = new Dictionary<string, object>
                    {
                        { "productionPerMinute", solarPanelSystemData.ProductionPerMinute}
                    };
                var tags = new Dictionary<string, string>
                    {
                        { "deviceId", solarPanelSystem.Id.ToString() }
                    };
                solarPanelSystemService.AddProductionMeasurement(fields, tags);
            }
        }

        public override Task<bool> ConnectToSmartDevice(SmartDevice smartDevice)
        {
            SolarPanelSystem solarPanelSystem = (SolarPanelSystem)smartDevice;
            Dictionary<string, object> additionalAttributes = new Dictionary<string, object>
                        {
                            { "area", solarPanelSystem.Area },
                            { "efficiency", solarPanelSystem.Efficiency }
                        };

            var requestBody = new
            {
                device_id = smartDevice.Id,
                smart_home_id = smartDevice.SmartHome.Id,
                device_category = smartDevice.Category.ToString(),
                device_type = smartDevice.Type.ToString(),
                host = configuration["MqttBroker:Host"],
                port = configuration["MqttBroker:Port"],
                keepalive = configuration["MqttBroker:Keepalive"],
                kwargs = additionalAttributes
            };
            return simualtionsHandler.AddDeviceToSimulator(requestBody);
        }
    }
}
