using IntelliHome_Backend.Features.Shared.DTOs;

namespace IntelliHome_Backend.Features.SPU.DTOs
{
    public class VehicleGateDTO : SmartDeviceDTO
    {
        public String CurrentLicencePlate { get; set; }
        public Boolean IsPublic { get; set; }
        public Boolean IsOpen { get; set; }
        public Boolean IsOpenByUser { get; set; }
        public Boolean IsEntering { get; set; }
        public string ActionBy { get; set; }
        public Double PowerPerHour { get; set; }
        public List<String> AllowedLicencePlates { get; set; }
    }
}
