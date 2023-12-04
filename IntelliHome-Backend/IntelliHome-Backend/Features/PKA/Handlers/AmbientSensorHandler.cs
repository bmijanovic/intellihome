﻿using Data.Context;
using Data.Models.PKA;
using Data.Models.Shared;
using IntelliHome_Backend.Features.PKA.DTOs;
using IntelliHome_Backend.Features.PKA.Handlers.Interfaces;
using IntelliHome_Backend.Features.PKA.Services.Interfaces;
using IntelliHome_Backend.Features.Shared.Handlers;
using IntelliHome_Backend.Features.Shared.Handlers.Interfaces;
using IntelliHome_Backend.Features.Shared.Services.Interfaces;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;

namespace IntelliHome_Backend.Features.PKA.Handlers
{
    public class AmbientSensorHandler : SmartDeviceHandler, IAmbientSensorHandler
    {
        public AmbientSensorHandler(IMqttService mqttService, IServiceProvider serviceProvider, ISimulationsHandler simualtionsHandler) 
            : base(mqttService, serviceProvider, simualtionsHandler)
        {
            this.mqttService.SubscribeAsync($"FromDevice/+/{SmartDeviceCategory.PKA}/{SmartDeviceType.AMBIENTSENSOR}/+", HandleMessageFromDevice);
        }

        protected async override Task HandleMessageFromDevice(MqttApplicationMessageReceivedEventArgs e)
        {
            Console.WriteLine(e.ApplicationMessage.ConvertPayloadToString());

            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<PostgreSqlDbContext>();
                var influxDbContext = scope.ServiceProvider.GetRequiredService<InfluxDbContext>();
                var ambientSensorService = scope.ServiceProvider.GetRequiredService<IAmbientSensorService>();

                Console.WriteLine(e.ApplicationMessage.Topic.Split('/')[4]);
                var ambientSensor = await ambientSensorService.Get(Guid.Parse(e.ApplicationMessage.Topic.Split('/')[4]));

                if (ambientSensor != null)
                {
                    var ambientSensorData = JsonConvert.DeserializeObject<AmbientSensorData>(e.ApplicationMessage.ConvertPayloadToString());

                    var ambientSensorDataInflux = new Dictionary<string, object>
                    {
                        { "temperature", ambientSensorData.Temperature },
                        { "humidity", ambientSensorData.Humidity },
                    };

                    var ambientSensorDataTags = new Dictionary<string, string>
                    {
                        { "deviceId", ambientSensor.Id.ToString() }
                    };

                    influxDbContext.WriteToInfluxAsync("ambient_sensor", ambientSensorDataInflux, ambientSensorDataTags).Wait();

                    ambientSensor.Temperature = ambientSensorData.Temperature;
                    ambientSensor.Humidity = ambientSensorData.Humidity;

                    ambientSensorService.Update(ambientSensor);

                }

                

            }   

            
        }
    }
}
