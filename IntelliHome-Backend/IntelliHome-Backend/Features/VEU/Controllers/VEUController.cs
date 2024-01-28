﻿using Data.Models.Shared;
using Data.Models.VEU;
using IntelliHome_Backend.Features.Home.Services.Interfaces;
using IntelliHome_Backend.Features.Shared.Services.Interfacted;
using IntelliHome_Backend.Features.VEU.DTOs;
using IntelliHome_Backend.Features.VEU.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntelliHome_Backend.Features.VEU.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class VEUController : ControllerBase
    {
        private readonly ISmartHomeService _smartHomeService;
        private readonly IBatterySystemService _batterySystemService;
        private readonly ISolarPanelSystemService _solarPanelSystemService;
        private readonly IVehicleChargerService _vehicleChargerService;
        private readonly IImageService _imageService;

        public VEUController(ISmartHomeService smartHomeService, IBatterySystemService batterySystemService,
                ISolarPanelSystemService solarPanelSystemService, IVehicleChargerService vehicleChargerService,
                IImageService imageService)
        {
            _smartHomeService = smartHomeService;
            _batterySystemService = batterySystemService;
            _solarPanelSystemService = solarPanelSystemService;
            _vehicleChargerService = vehicleChargerService;
            _imageService = imageService;
        }

        [HttpPost]
        [Route("{smartHomeId:Guid}")]
        [Authorize]
        public async Task<ActionResult> CreateBatterySystem([FromRoute] Guid smartHomeId, [FromForm] BatterySystemCreationDTO dto)
        {
            BatterySystem batterySystem = new BatterySystem
            {
                SmartHome = await _smartHomeService.Get(smartHomeId),
                Name = dto.Name,
                Category = SmartDeviceCategory.VEU,
                Type = SmartDeviceType.BATTERYSYSTEM,
                Capacity = dto.Capacity,
                Image = dto.Image != null && dto.Image.Length > 0 ? _imageService.SaveDeviceImage(dto.Image) : null
            };
            batterySystem = await _batterySystemService.Create(batterySystem);
            return Ok(batterySystem);
        }

        [HttpPost]
        [Route("{smartHomeId:Guid}")]
        [Authorize]
        public async Task<ActionResult> CreateSolarPanelSystem([FromRoute] Guid smartHomeId, [FromForm] SolarPanelSystemCreationDTO dto)
        {
            SolarPanelSystem solarPanelSystem = new SolarPanelSystem
            {
                SmartHome = await _smartHomeService.Get(smartHomeId),
                Name = dto.Name,
                Category = SmartDeviceCategory.VEU,
                Type = SmartDeviceType.SOLARPANELSYSTEM,
                Area = dto.Area,
                Efficiency = dto.Efficiency,
                Image = dto.Image != null && dto.Image.Length > 0 ? _imageService.SaveDeviceImage(dto.Image) : null
            };
            solarPanelSystem = await _solarPanelSystemService.Create(solarPanelSystem);
            return Ok(solarPanelSystem);
        }

        [HttpPost]
        [Route("{smartHomeId:Guid}")]
        [Authorize]
        public async Task<ActionResult> CreateVehicleCharger([FromRoute] Guid smartHomeId, [FromForm] VehicleChargerCreationDTO dto)
        {
            VehicleCharger vehicleCharger = new VehicleCharger
            {
                SmartHome = await _smartHomeService.Get(smartHomeId),
                Name = dto.Name,
                Category = SmartDeviceCategory.VEU,
                Type = SmartDeviceType.VEHICLECHARGER,
                PowerPerHour = dto.PowerPerHour,
                ChargingPoints = Enumerable.Range(0, dto.NumberOfChargingPoints).Select(_ => new VehicleChargingPoint { IsFree = true }).ToList(),
                Image = dto.Image != null && dto.Image.Length > 0 ? _imageService.SaveDeviceImage(dto.Image) : null
            };
            vehicleCharger = await _vehicleChargerService.Create(vehicleCharger);
            return Ok(vehicleCharger);
        }
    }
}
