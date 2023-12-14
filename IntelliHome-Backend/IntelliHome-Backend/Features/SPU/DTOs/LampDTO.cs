using IntelliHome_Backend.Features.Shared.DTOs;

namespace IntelliHome_Backend.Features.SPU.DTOs
{
    public class LampDTO : SmartDeviceDTO
    {
        public Double CurrentBrightness { get; set; }
        public Double BrightnessLimit { get; set; }
        public Boolean IsWorking { get; set; }
        public Boolean IsAuto { get; set; }
        public Double PowerPerHour { get; set; }
    }
}
