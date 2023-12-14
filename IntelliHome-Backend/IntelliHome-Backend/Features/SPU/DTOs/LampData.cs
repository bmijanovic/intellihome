namespace IntelliHome_Backend.Features.SPU.DTOs
{
    public class LampData
    {
        public Double CurrentBrightness { get; set; }
        public Boolean IsWorking { get; set; }
        public Boolean IsAuto { get; set; }
        public Double ConsumptionPerMinute { get; set; }
        public DateTime? Timestamp { get; set; }
    }
}
