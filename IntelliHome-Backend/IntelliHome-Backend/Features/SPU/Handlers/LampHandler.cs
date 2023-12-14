using IntelliHome_Backend.Features.Shared.Handlers.Interfaces;
using IntelliHome_Backend.Features.Shared.Services.Interfaces;
using IntelliHome_Backend.Features.SPU.Handlers.Interfaces;
using MQTTnet.Client;
using MQTTnet;
using Data.Models.Shared;
using Data.Models.SPU;
using IntelliHome_Backend.Features.Shared.Hubs.Interfaces;
using IntelliHome_Backend.Features.Shared.Hubs;
using IntelliHome_Backend.Features.SPU.DTOs;
using IntelliHome_Backend.Features.SPU.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using SendGrid.Helpers.Errors.Model;
using IntelliHome_Backend.Features.Home.Handlers;

namespace IntelliHome_Backend.Features.SPU.Handlers
{
    public class LampHandler : SmartDeviceHandler, ILampHandler
    {
        public LampHandler(IMqttService mqttService, IServiceProvider serviceProvider, ISimulationsHandler simualtionsHandler, IHubContext<SmartDeviceHub, ISmartDeviceClient> smartDeviceHubContext)
            : base(mqttService, serviceProvider, simualtionsHandler, smartDeviceHubContext)
        {
            this.mqttService.SubscribeAsync($"FromDevice/+/{SmartDeviceCategory.SPU}/{SmartDeviceType.LAMP}/+", HandleMessageFromDevice);
        }

        protected override async Task HandleMessageFromDevice(MqttApplicationMessageReceivedEventArgs e)
        {
            Console.WriteLine(e.ApplicationMessage.ConvertPayloadToString());
            Console.WriteLine(e.ApplicationMessage.Topic.Split("/").Last());
            smartDeviceHubContext.Clients.Group(e.ApplicationMessage.Topic.Split("/").Last()).ReceiveSmartDeviceData(e.ApplicationMessage.ConvertPayloadToString());

            using var scope = serviceProvider.CreateScope();
            
            var lampService = scope.ServiceProvider.GetRequiredService<ILampService>();
            
            var lamp = await lampService.Get(Guid.Parse(e.ApplicationMessage.Topic.Split('/')[4]));
            
            if (lamp != null)
            {
                var lampData = JsonConvert.DeserializeObject<LampData>(e.ApplicationMessage.ConvertPayloadToString() );
                var lampDataInflux = new Dictionary<string, object>
                {
                        { "currentBrightness", lampData.CurrentBrightness },
                        { "isWorking", lampData.IsWorking ? 1f : 0f },
                        { "isAuto", lampData.IsAuto ? 1f : 0f},
                        { "consumptionPerMinute", lampData.ConsumptionPerMinute }
            
                };
                var lampDataTags = new Dictionary<string, string>
                {
                        { "deviceId", lamp.Id.ToString() }
                };
                lampService.AddPoint(lampDataInflux, lampDataTags);
            }


        }

        public void ChangeMode(Lamp lamp, bool isAuto)
        {
            string action = isAuto ? "auto" : "manual";
            string payload = JsonConvert.SerializeObject(new { action });
            PublishMessageToSmartDevice(lamp, payload);
        }

        public void ChangeBrightnessLimit(Lamp lamp, double brightness)
        {
            string action = $"set_brightness_limit={brightness}";
            string payload = JsonConvert.SerializeObject(new { action });
            PublishMessageToSmartDevice(lamp, payload);
        }

        public void TurnLampOn(Lamp lamp, Boolean isOn)
        {
            string action = isOn ? "turn_lamp_on" : "turn_lamp_off";
            string payload = JsonConvert.SerializeObject(new { action });
            PublishMessageToSmartDevice(lamp, payload);
        }
    }
}
