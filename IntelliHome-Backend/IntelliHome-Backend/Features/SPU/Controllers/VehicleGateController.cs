using IntelliHome_Backend.Features.SPU.DTOs;
using IntelliHome_Backend.Features.SPU.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IntelliHome_Backend.Features.SPU.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class  VehicleGateController : ControllerBase
    {
        private readonly IVehicleGateService _vehicleGateService;

        public VehicleGateController(IVehicleGateService vehicleGateService)
        {
            _vehicleGateService = vehicleGateService;
        }

        [HttpGet]
        public async Task<IActionResult> GetHistoricalData(Guid id, DateTime from, DateTime to)
        {
            List<VehicleGateData> result = _vehicleGateService.GetHistoricalData(id, from, to);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid id)
        {
            VehicleGateDTO result = await _vehicleGateService.GetWithData(id);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> ChangeMode(Guid id, Boolean isPublic)
        {
            await _vehicleGateService.ChangeMode(id, isPublic);
            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> TurnOnSmartDevice(Guid id, bool turnOn)
        {
            await _vehicleGateService.TurnOnSmartDevice(id, turnOn);
            return Ok();
        }

        
        [HttpPut]
        public async Task<ActionResult> AddLicencePlate(Guid id, string licencePlate)
        {
            await _vehicleGateService.AddLicencePlate(id, licencePlate);
            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> RemoveLicencePlate(Guid id, string licencePlate)
        {
            await _vehicleGateService.RemoveLicencePlate(id, licencePlate);
            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> OpenCloseGate(Guid id, bool isOpen)
        {
            AuthenticateResult result = await HttpContext.AuthenticateAsync();
            if (!result.Succeeded)
            {
                return BadRequest("Cookie error");
            }
            ClaimsIdentity identity = result.Principal.Identity as ClaimsIdentity;
            string username = identity.FindFirst(ClaimTypes.Name).Value;
            await _vehicleGateService.OpenCloseGate(id, isOpen, username);
            return Ok();
        }
    }
}
