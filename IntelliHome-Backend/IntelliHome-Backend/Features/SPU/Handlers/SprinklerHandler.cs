﻿using IntelliHome_Backend.Features.Shared.Handlers.Interfaces;
using IntelliHome_Backend.Features.Shared.Handlers;
using IntelliHome_Backend.Features.Shared.Services.Interfaces;
using IntelliHome_Backend.Features.SPU.Handlers.Interfaces;
using MQTTnet.Client;
using MQTTnet;
using Data.Models.Shared;

namespace IntelliHome_Backend.Features.SPU.Handlers
{
    public class SprinklerHandler : SmartDeviceHandler, ISprinklerHandler
    {
        public SprinklerHandler(IMqttService mqttService, IServiceProvider serviceProvider, ISimulationsHandler simualtionsHandler)
            : base(mqttService, serviceProvider, simualtionsHandler)
        {
            this.mqttService.SubscribeAsync($"FromDevice/+/{SmartDeviceCategory.SPU}/{SmartDeviceType.SPRINKLER}/+", HandleMessageFromDevice);
        }

        protected override Task HandleMessageFromDevice(MqttApplicationMessageReceivedEventArgs e)
        {
            Console.WriteLine(e.ApplicationMessage.ConvertPayloadToString());
            return Task.CompletedTask;
        }
    }
}
