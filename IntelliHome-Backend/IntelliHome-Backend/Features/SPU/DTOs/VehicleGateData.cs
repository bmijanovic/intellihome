namespace IntelliHome_Backend.Features.SPU.DTOs
{
    public class VehicleGateData
    {
        public String LicencePlate { get; set; }
        public Boolean IsPublic { get; set; }
        public Boolean IsOpen { get; set; }
        public Boolean IsOpenByUser { get; set; }
        public Boolean IsEntering { get; set; }
        public Double ConsumptionPerMinute { get; set; }
        public String ActionBy { get; set; }
        public DateTime? Timestamp { get; set; }
    }
}
