﻿using Data.Models.VEU;
using IntelliHome_Backend.Features.Shared.DTOs;
using IntelliHome_Backend.Features.VEU.DTOs.VehicleCharger;
using IntelliHome_Backend.Features.VEU.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IntelliHome_Backend.Features.VEU.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class VehicleChargerController : ControllerBase
    {
        private readonly IVehicleChargerService _vehicleChargerService;

        public VehicleChargerController(IVehicleChargerService vehicleChargerService)
        {
            _vehicleChargerService = vehicleChargerService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get(Guid id)
        {
            VehicleChargerDTO result = await _vehicleChargerService.GetWithChargingPointsData(id);
            return Ok(result);
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult> Toggle(Guid id, bool turnOn = true)
        {
            AuthenticateResult result = await HttpContext.AuthenticateAsync();
            if (!result.Succeeded)
            {
                return BadRequest("Cookie error");
            }
            ClaimsIdentity identity = result.Principal.Identity as ClaimsIdentity;
            string username = identity.FindFirst(ClaimTypes.Name).Value;
            await _vehicleChargerService.Toggle(id, username,turnOn);
            return Ok();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetActionHistoricalData(Guid id, DateTime from, DateTime to)
        {
            if (from > to)
            {
                return BadRequest("FROM date cant be after TO date");
            }
            List<ActionDataDTO> result = _vehicleChargerService.GetActionHistoricalData(id, from, to);
            return Ok(result);
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult> ConnectToCharger(Guid vehicleChargerId, Guid vehicleChargingPointId, VehicleChargingPointCreationDTO vehicleChargingPointCreationDTO)
        {
            VehicleChargingPoint vehicleChargingPoint = new VehicleChargingPoint
            {
                Id = vehicleChargingPointId,
                ChargeLimit = vehicleChargingPointCreationDTO.ChargeLimit,
                IsFree = false,
                Capacity = vehicleChargingPointCreationDTO.Capacity,
                InitialCapacity = vehicleChargingPointCreationDTO.InitialCapacity
            };
            VehicleCharger vehicleCharger = await _vehicleChargerService.ConnectToCharger(vehicleChargerId, vehicleChargingPoint);
            return Ok("Successfully connected to charger (charging point)!");
        }


        [HttpPut]
        [Authorize]
        public async Task<ActionResult> DisconnectFromCharger(Guid vehicleChargerId, Guid vehicleChargingPointId)
        {
            await _vehicleChargerService.DisconnectFromCharger(vehicleChargerId, vehicleChargingPointId);
            return Ok("Successfully disconnected from charger (charging point)!");
        }
    }
}
