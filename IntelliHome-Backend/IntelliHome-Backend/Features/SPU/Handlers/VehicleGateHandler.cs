using IntelliHome_Backend.Features.Shared.Handlers.Interfaces;
using IntelliHome_Backend.Features.Shared.Services.Interfaces;
using IntelliHome_Backend.Features.SPU.Handlers.Interfaces;
using MQTTnet.Client;
using MQTTnet;
using Data.Models.Shared;
using IntelliHome_Backend.Features.Shared.Hubs.Interfaces;
using IntelliHome_Backend.Features.Shared.Hubs;
using Microsoft.AspNetCore.SignalR;
using IntelliHome_Backend.Features.Home.Handlers;
using Data.Models.SPU;
using IntelliHome_Backend.Features.SPU.DTOs;
using IntelliHome_Backend.Features.SPU.Services.Interfaces;
using Newtonsoft.Json;

namespace IntelliHome_Backend.Features.SPU.Handlers
{
    public class VehicleGateHandler : SmartDeviceHandler, IVehicleGateHandler
    {
        public VehicleGateHandler(IMqttService mqttService, IServiceProvider serviceProvider, ISimulationsHandler simualtionsHandler, IHubContext<SmartDeviceHub, ISmartDeviceClient> smartDeviceHubContext)
            : base(mqttService, serviceProvider, simualtionsHandler, smartDeviceHubContext)
        {
            this.mqttService.SubscribeAsync($"FromDevice/+/{SmartDeviceCategory.SPU}/{SmartDeviceType.VEHICLEGATE}/+", HandleMessageFromDevice);
        }

        protected override async Task HandleMessageFromDevice(MqttApplicationMessageReceivedEventArgs e)
        {
            Console.WriteLine(e.ApplicationMessage.ConvertPayloadToString());
            smartDeviceHubContext.Clients.Group(e.ApplicationMessage.Topic.Split("/").Last()).ReceiveSmartDeviceData(e.ApplicationMessage.ConvertPayloadToString());

            using var scope = serviceProvider.CreateScope();

            var vehicleGateService = scope.ServiceProvider.GetRequiredService<IVehicleGateService>();

            var vehicleGate = await vehicleGateService.Get(Guid.Parse(e.ApplicationMessage.Topic.Split('/')[4]));

            if (vehicleGate != null)
            {
                var vehicleGateData = JsonConvert.DeserializeObject<VehicleGateData>(e.ApplicationMessage.ConvertPayloadToString());
                var vehicleGateDataInflux = new Dictionary<string, object>
                {
                        { "isPublic", vehicleGateData.IsPublic ? 1f : 0f },
                        { "isOpen", vehicleGateData.IsOpen ? 1f : 0f },
                        { "isEntering", vehicleGateData.IsEntering ? 1f : 0f },
                        { "isOpenByUser", vehicleGateData.IsOpenByUser ? 1f : 0f},
                        { "consumptionPerMinute", vehicleGateData.ConsumptionPerMinute }

                };
                var vehicleGateDataTags = new Dictionary<string, string>
                {
                        { "actionBy", vehicleGateData.ActionBy},
                        { "licencePlate", vehicleGateData.LicencePlate },
                        { "deviceId", vehicleGate.Id.ToString() }
                };
                vehicleGateService.AddPoint(vehicleGateDataInflux, vehicleGateDataTags);
            }
        }

        public void ChangeMode(VehicleGate vehicle, bool isPublic)
        {
            string action = isPublic ? "public" : "private";
            string payload = JsonConvert.SerializeObject(new { action });

            PublishMessageToSmartDevice(vehicle, payload);
        }

        public void AddLicencePlate(VehicleGate vehicleGate, string licencePlate)
        {
            string action = $"add_licence_plate";
            string licence_plate = licencePlate;
            string payload = JsonConvert.SerializeObject(new { action, licence_plate });
            PublishMessageToSmartDevice(vehicleGate, payload);
        }

        public void RemoveLicencePlate(VehicleGate vehicleGate, string licencePlate)
        {
            string action = $"remove_licence_plate";
            string licence_plate = licencePlate;
            string payload = JsonConvert.SerializeObject(new { action, licence_plate });
            PublishMessageToSmartDevice(vehicleGate, payload);
        }

        public void OpenCloseGate(VehicleGate vehicleGate, bool isOpen, string username)
        {
            string action = isOpen ? "open_by_user" : "close_by_user";
            string user = username;
            string payload = JsonConvert.SerializeObject(new { action, user });
            PublishMessageToSmartDevice(vehicleGate, payload);
        }
    }
}
