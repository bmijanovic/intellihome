﻿using Data.Models.Shared;
using IntelliHome_Backend.Features.Home.Services.Interfaces;
using IntelliHome_Backend.Features.Shared.Handlers.Interfaces;
using IntelliHome_Backend.Features.Shared.Hubs.Interfaces;
using IntelliHome_Backend.Features.Shared.Hubs;
using IntelliHome_Backend.Features.Shared.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using IntelliHome_Backend.Features.Shared.Services;

namespace IntelliHome_Backend.Features.Shared.Handlers
{
    public class LastWillHandler : ILastWillHandler
    {
        private readonly IMqttService _mqttService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHubContext<SmartDeviceHub, ISmartDeviceClient> _smartDeviceHubContext;
        public LastWillHandler(IConfiguration configuration, MqttFactory mqttFactory, IServiceProvider serviceProvider, IHubContext<SmartDeviceHub, ISmartDeviceClient> smartDeviceHubContext)
        {
            _mqttService = new MqttService(mqttFactory);
            _mqttService.ConnectAsync(configuration["MqttBroker:Host"], Convert.ToInt32(configuration["MqttBroker:Port"])).Wait();
            _serviceProvider = serviceProvider;
            _smartDeviceHubContext = smartDeviceHubContext;
        }

        public async Task SetupLastWillHandler()
        {
            await _mqttService.SubscribeAsync("will", HandleLastWillMessageAsync);
        }

        private async Task HandleLastWillMessageAsync(MqttApplicationMessageReceivedEventArgs e)
        {
            using var scope = _serviceProvider.CreateScope();
            ISmartDeviceService smartDeviceService = scope.ServiceProvider.GetRequiredService<ISmartDeviceService>();
            Guid deviceId = Guid.Parse(e.ApplicationMessage.ConvertPayloadToString());
            SmartDevice smartDevice = await smartDeviceService.Get(deviceId);
            smartDevice.IsConnected = false;
            smartDevice.IsOn = false;
            await smartDeviceService.Update(smartDevice);
            smartDeviceService.UpdateAvailability(new List<Guid> { deviceId }, false);
            _smartDeviceHubContext.Clients.Group(deviceId.ToString()).ReceiveSmartDeviceData(JsonConvert.SerializeObject(new { isConnected = smartDevice.IsConnected }));
        }

    }
}
