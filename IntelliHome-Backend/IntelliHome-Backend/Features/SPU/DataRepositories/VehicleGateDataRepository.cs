using InfluxDB.Client.Core.Flux.Domain;
using IntelliHome_Backend.Features.Shared.Influx;
using IntelliHome_Backend.Features.SPU.DataRepositories.Interfaces;
using IntelliHome_Backend.Features.SPU.DTOs;

namespace IntelliHome_Backend.Features.SPU.DataRepositories
{
    public class VehicleGateDataRepository : IVehicleGateDataRepository
    {

        private readonly InfluxRepository _influxRepository;

        public VehicleGateDataRepository(InfluxRepository influxRepository)
        {
            _influxRepository = influxRepository;
        }

        public VehicleGateData GetLastData(Guid id)
        {
            var table = _influxRepository.GetLastData("vehicleGate", id).Result;
            if (table == null || table.Records.Count == 0)
            {
                return new VehicleGateData
                {
                    IsOpen = false,
                    IsPublic = false,
                    IsEntering = false,
                    LicencePlate = "",
                };
            }
            VehicleGateData vehicleGateData = ConvertToVehicleGateData(table);

            return new VehicleGateData
            {
                IsOpen = vehicleGateData.IsOpen,
                IsPublic = vehicleGateData.IsPublic,
                IsEntering = vehicleGateData.IsEntering,
                LicencePlate = vehicleGateData.LicencePlate,
            };

        }

        public List<VehicleGateData> GetHistoricalData(Guid id, DateTime from, DateTime to)
        {
            var result = _influxRepository.GetHistoricalData("vehicleGate", id, from, to).Result;
            return result.Select(ConvertToVehicleGateData).ToList();
        }

        public void AddPoint(Dictionary<string, object> fields, Dictionary<string, string> tags)
        {
            _influxRepository.WriteToInfluxAsync("vehicleGate", fields, tags);
        }

        private VehicleGateData ConvertToVehicleGateData(FluxTable table)
        {
            var rows = table.Records;
            DateTime timestamp = DateTime.Parse(rows[0].GetValueByKey("_time").ToString());
            TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
            timestamp = TimeZoneInfo.ConvertTime(timestamp, localTimeZone);

            var isOpenRecord = rows.FirstOrDefault(r => r.Row.Contains("isOpen"));
            var isPublicRecord = rows.FirstOrDefault(r => r.Row.Contains("isPublic"));
            var isEnteringRecord = rows.FirstOrDefault(r => r.Row.Contains("isEntering"));
            var isOpenByUserRecord = rows.FirstOrDefault(r => r.Row.Contains("isOpenByUser"));

            bool isOpen = isOpenRecord != null && Convert.ToBoolean(isOpenRecord.GetValueByKey("_value"));
            bool isPublic = isPublicRecord != null && Convert.ToBoolean(isPublicRecord.GetValueByKey("_value"));
            bool isEntering = isEnteringRecord != null && Convert.ToBoolean(isEnteringRecord.GetValueByKey("_value"));
            string licencePlate = rows[0].GetValueByKey("licencePlate") != null ? rows[0].GetValueByKey("licencePlate").ToString() : "";
            string actionBy = rows[0].GetValueByKey("actionBy") != null ? rows[0].GetValueByKey("actionBy").ToString() : "";
            bool isOpenByUser = isOpenByUserRecord != null && Convert.ToBoolean(isOpenByUserRecord.GetValueByKey("_value"));
            

            return new VehicleGateData
            {
                Timestamp = timestamp,
                IsOpen = isOpen,
                IsPublic = isPublic,
                IsEntering = isEntering,
                LicencePlate = licencePlate,
                IsOpenByUser = isOpenByUser,
                ActionBy = actionBy,
            };
        }
    }
}
