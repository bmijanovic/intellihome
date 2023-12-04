﻿using Data.Models.Shared;
using Data.Models.SPU;
using IntelliHome_Backend.Features.Home.Services.Interfaces;
using IntelliHome_Backend.Features.Shared.Services.Interfacted;
using IntelliHome_Backend.Features.SPU.DTOs;
using IntelliHome_Backend.Features.SPU.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IntelliHome_Backend.Features.SPU.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SPUController : ControllerBase
    {
        private readonly ISmartHomeService _smartHomeService;
        private readonly ILampService _lampService;
        private readonly IVehicleGateService _vehicleGateService;
        private readonly ISprinklerService _sprinklerService;
        private readonly IImageService _imageService;

        public SPUController(ISmartHomeService smartHomeService, ILampService lampService,
            IVehicleGateService vehicleGateService, ISprinklerService sprinklerService,
            IImageService imageService)
        {
            _smartHomeService = smartHomeService;
            _lampService = lampService;
            _vehicleGateService = vehicleGateService;
            _sprinklerService = sprinklerService;
            _imageService = imageService;
        }

        [HttpPost]
        [Route("{smartHomeId:Guid}")]
        public async Task<ActionResult> CreateLamp([FromRoute] Guid smartHomeId, [FromForm] LampCreationDTO dto)
        {
            Lamp lamp = new Lamp
            {
                SmartHome = await _smartHomeService.Get(smartHomeId),
                Name = dto.Name,
                Category = SmartDeviceCategory.SPU,
                Type = SmartDeviceType.LAMP,
                PowerPerHour = dto.PowerPerHour,
                BrightnessLimit = dto.BrightnessLimit,
                Image = dto.Image != null && dto.Image.Length > 0 ? _imageService.SaveDeviceImage(dto.Image) : null
            };
            lamp = await _lampService.Create(lamp);
            return Ok(lamp);

        }

        [HttpPost]
        [Route("{smartHomeId:Guid}")]
        public async Task<ActionResult> CreateSprinkler([FromRoute] Guid smartHomeId, [FromForm] SprinklerCreationDTO dto)
        {
            Sprinkler sprinkler = new Sprinkler
            {
                SmartHome = await _smartHomeService.Get(smartHomeId),
                Name = dto.Name,
                Category = SmartDeviceCategory.SPU,
                Type = SmartDeviceType.SPRINKLER,
                PowerPerHour = dto.PowerPerHour,
                Image = dto.Image != null && dto.Image.Length > 0 ? _imageService.SaveDeviceImage(dto.Image) : null
            };
            sprinkler = await _sprinklerService.Create(sprinkler);
            return Ok(sprinkler);

        }

        [HttpPost]
        [Route("{smartHomeId:Guid}")]
        public async Task<ActionResult> CreateVehicleGate([FromRoute] Guid smartHomeId, [FromForm] VehicleGateCreationDTO dto)
        {
            VehicleGate vehicleGate = new VehicleGate
            {
                SmartHome = await _smartHomeService.Get(smartHomeId),
                Name = dto.Name,
                Category = SmartDeviceCategory.SPU,
                Type = SmartDeviceType.VEHICLEGATE,
                PowerPerHour = dto.PowerPerHour,
                AllowedLicencePlates = dto.AllowedLicencePlates,
                Image = dto.Image != null && dto.Image.Length > 0 ? _imageService.SaveDeviceImage(dto.Image) : null
            };
            vehicleGate = await _vehicleGateService.Create(vehicleGate);
            return Ok(vehicleGate);

        }
    }
}