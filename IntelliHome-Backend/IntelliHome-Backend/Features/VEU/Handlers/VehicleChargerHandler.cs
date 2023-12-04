﻿using IntelliHome_Backend.Features.Shared.Handlers.Interfaces;
using IntelliHome_Backend.Features.Shared.Handlers;
using IntelliHome_Backend.Features.Shared.Services.Interfaces;
using IntelliHome_Backend.Features.VEU.Handlers.Interfaces;
using MQTTnet.Client;
using MQTTnet;
using Data.Models.Shared;

namespace IntelliHome_Backend.Features.VEU.Handlers
{
    public class VehicleChargerHandler : SmartDeviceHandler, IVehicleChargerHandler
    {
        public VehicleChargerHandler(IMqttService mqttService, IServiceProvider serviceProvider, ISimulationsHandler simualtionsHandler)
            : base(mqttService, serviceProvider, simualtionsHandler)
        {
            this.mqttService.SubscribeAsync($"FromDevice/+/{SmartDeviceCategory.VEU}/{SmartDeviceType.VEHICLECHARGER}/+", HandleMessageFromDevice);
        }

        protected override Task HandleMessageFromDevice(MqttApplicationMessageReceivedEventArgs e)
        {
            Console.WriteLine(e.ApplicationMessage.ConvertPayloadToString());
            return Task.CompletedTask;
        }
    }
}