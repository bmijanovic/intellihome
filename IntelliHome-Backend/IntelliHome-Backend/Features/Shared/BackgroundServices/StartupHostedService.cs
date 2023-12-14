using Data.Models.PKA;
using Data.Models.Shared;
using Data.Models.SPU;
using Data.Models.VEU;
using IntelliHome_Backend.Features.Home.Handlers.Interfaces;
using IntelliHome_Backend.Features.Home.Services.Interfaces;
using IntelliHome_Backend.Features.PKA.Handlers.Interfaces;
using IntelliHome_Backend.Features.Shared.Handlers.Interfaces;
using IntelliHome_Backend.Features.SPU.Handlers.Interfaces;
using IntelliHome_Backend.Features.VEU.Handlers.Interfaces;

namespace IntelliHome_Backend.Features.Shared.BackgroundServices
{
    public class StartupHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        public StartupHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                ILastWillHandler heartbeatService = scope.ServiceProvider.GetRequiredService<ILastWillHandler>();
                ISimulationsHandler simulationService = scope.ServiceProvider.GetRequiredService<ISimulationsHandler>();

                //eliminsanje lazy loadinga
                ISmartHomeHandler smartHomeHandler = scope.ServiceProvider.GetRequiredService<ISmartHomeHandler>();

                Task.Run(() => SetupDevicesFromDatabase());
                Task.Run(() => heartbeatService.SetupLastWillHandler());
                return Task.CompletedTask;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                ISimulationsHandler simulationService = scope.ServiceProvider.GetRequiredService<ISimulationsHandler>();
                //TODO stop all simulations
                return Task.CompletedTask;
            }
        }

        public async Task SetupDevicesFromDatabase()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                ISmartDeviceService smartDeviceService = scope.ServiceProvider.GetRequiredService<ISmartDeviceService>();
                IAmbientSensorHandler ambientSensorHandler = scope.ServiceProvider.GetRequiredService<IAmbientSensorHandler>();
                IAirConditionerHandler airConditionerHandler = scope.ServiceProvider.GetRequiredService<IAirConditionerHandler>();
                IWashingMachineHandler washingMachineHandler = scope.ServiceProvider.GetRequiredService<IWashingMachineHandler>();
                ILampHandler lampHandler = scope.ServiceProvider.GetRequiredService<ILampHandler>();
                IVehicleGateHandler vehicleGateHandler = scope.ServiceProvider.GetRequiredService<IVehicleGateHandler>();
                ISprinklerHandler sprinklerHandler = scope.ServiceProvider.GetRequiredService<ISprinklerHandler>();
                IBatterySystemHandler batterySystemHandler = scope.ServiceProvider.GetRequiredService<IBatterySystemHandler>();
                ISolarPanelSystemHandler solarPanelSystemHandler = scope.ServiceProvider.GetRequiredService<ISolarPanelSystemHandler>();
                IVehicleChargerHandler vehicleChargerHandler = scope.ServiceProvider.GetRequiredService<IVehicleChargerHandler>();

                List<SmartDevice> smartDevices = smartDeviceService.GetAllWithHome().ToList();
                foreach (SmartDevice smartDevice in smartDevices)
                {
                    if (smartDevice.Type == SmartDeviceType.AMBIENTSENSOR)
                    {
                        AmbientSensor ambientSensor = (AmbientSensor)smartDevice;
                        Dictionary<string, object> additionalAttributes = new Dictionary<string, object>
                        {
                            { "power_per_hour", ambientSensor.PowerPerHour},
                        };
                        smartDevice.IsConnected = await ambientSensorHandler.ConnectToSmartDevice(smartDevice, additionalAttributes);
                    }
                    else if (smartDevice.Type == SmartDeviceType.AIRCONDITIONER)
                    {
                        AirConditioner airConditioner = (AirConditioner)smartDevice;
                        Dictionary<string, object> additionalAttributes = new Dictionary<string, object>
                        {
                            { "power_per_hour", airConditioner.PowerPerHour},
                        };
                        smartDevice.IsConnected = await airConditionerHandler.ConnectToSmartDevice(smartDevice, additionalAttributes);
                    }
                    else if (smartDevice.Type == SmartDeviceType.WASHINGMACHINE)
                    {
                        WashingMachine washingMachine = (WashingMachine)smartDevice;
                        Dictionary<string, object> additionalAttributes = new Dictionary<string, object>
                        {
                            { "power_per_hour", washingMachine.PowerPerHour},
                        };
                        smartDevice.IsConnected = await washingMachineHandler.ConnectToSmartDevice(smartDevice, additionalAttributes);
                    }

                    else if (smartDevice.Type == SmartDeviceType.LAMP)
                    {
                        Lamp lamp = (Lamp)smartDevice;
                        Dictionary<string, object> additionalAttributes = new Dictionary<string, object>
                        {
                            { "brightness_limit", lamp.BrightnessLimit },
                            { "is_working", lamp.IsOn},
                            { "is_auto", lamp.IsAuto},
                            { "power_per_hour", lamp.PowerPerHour},
                        };
                        smartDevice.IsConnected = await lampHandler.ConnectToSmartDevice(smartDevice, additionalAttributes);
                    }
                    else if (smartDevice.Type == SmartDeviceType.VEHICLEGATE)
                    {
                        VehicleGate vehicleGate = (VehicleGate)smartDevice;
                        Dictionary<string, object> additionalAttributes = new Dictionary<string, object>
                        {
                            { "is_public", vehicleGate.IsPublic },
                            { "allowed_licence_plates", vehicleGate.AllowedLicencePlates },
                            { "power_per_hour", vehicleGate.PowerPerHour }
                        };
                        smartDevice.IsConnected = await vehicleGateHandler.ConnectToSmartDevice(smartDevice, additionalAttributes);
                    }
                    else if (smartDevice.Type == SmartDeviceType.SPRINKLER)
                    {
                        Sprinkler sprinkler = (Sprinkler)smartDevice;
                        Dictionary<string, object> additionalAttributes = new Dictionary<string, object>
                        {
                            { "power_per_hour", sprinkler.PowerPerHour},
                        };
                        smartDevice.IsConnected = await sprinklerHandler.ConnectToSmartDevice(smartDevice, additionalAttributes);
                    }

                    else if (smartDevice.Type == SmartDeviceType.BATTERYSYSTEM)
                    {
                        BatterySystem batterySystem = (BatterySystem)smartDevice;
                        Dictionary<string, object> additionalAttributes = new Dictionary<string, object>
                        {
                            { "capacity", batterySystem.Capacity }
                        };
                        smartDevice.IsConnected = await batterySystemHandler.ConnectToSmartDevice(smartDevice, additionalAttributes);
                    }
                    else if (smartDevice.Type == SmartDeviceType.SOLARPANELSYSTEM)
                    {
                        SolarPanelSystem solarPanelSystem = (SolarPanelSystem)smartDevice;
                        Dictionary<string, object> additionalAttributes = new Dictionary<string, object>
                        {
                            { "area", solarPanelSystem.Area },
                            { "efficiency", solarPanelSystem.Efficiency }
                        };
                        smartDevice.IsConnected = await solarPanelSystemHandler.ConnectToSmartDevice(smartDevice, additionalAttributes);
                    }
                    else if (smartDevice.Type == SmartDeviceType.VEHICLECHARGER)
                    {
                        smartDevice.IsConnected = await vehicleChargerHandler.ConnectToSmartDevice(smartDevice, new Dictionary<string, object>());
                    }
                }
                smartDeviceService.UpdateAll(smartDevices);
            }
        }
    }
}
