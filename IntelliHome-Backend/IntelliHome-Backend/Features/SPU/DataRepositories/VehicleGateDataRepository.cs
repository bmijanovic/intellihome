﻿using InfluxDB.Client.Core.Flux.Domain;
using IntelliHome_Backend.Features.Shared.DTOs;
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
            return table == null || table.Records.Count == 0 ? new VehicleGateData() : ConvertToVehicleGateData(table);

        }

        public List<VehicleGateData> GetHistoricalData(Guid id, DateTime from, DateTime to)
        {
            var result = _influxRepository.GetHistoricalData("vehicleGate", id, from, to).Result;
            return result.Select(ConvertToVehicleGateData).ToList();
        }

        public List<ActionDataDTO> GetHistoricalActionData(Guid id, DateTime from, DateTime to)
        {
            var result = _influxRepository.GetHistoricalData("vehicleGateAction", id, from, to).Result;
            return result.Select(ConvertToVehicleGateActionData).ToList();
        }

        public void AddPoint(Dictionary<string, object> fields, Dictionary<string, string> tags)
        {
            _influxRepository.WriteToInfluxAsync("vehicleGate", fields, tags);
        }

        public void SaveAction(Dictionary<string, object> fields, Dictionary<string, string> tags)
        {
            _influxRepository.WriteToInfluxAsync("vehicleGateAction", fields, tags);
        }

        private ActionDataDTO ConvertToVehicleGateActionData(FluxTable table)
        {
            var rows = table.Records;
            DateTime timestamp = DateTime.Parse(rows[0].GetValueByKey("_time").ToString());
            TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
            timestamp = TimeZoneInfo.ConvertTime(timestamp, localTimeZone);

            var actionRecord = rows.FirstOrDefault(r => r.Row.Contains("action"));

            string action = actionRecord != null ? actionRecord.GetValueByKey("_value").ToString() : "";
            string actionBy = rows[0].GetValueByKey("username") != null ? rows[0].GetValueByKey("username").ToString() : "";

            return new ActionDataDTO
            {
                Timestamp = timestamp,
                Action = action,
                ActionBy = actionBy
            };

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
            var isOpendByUserRecord = rows.FirstOrDefault(r => r.Row.Contains("isOpenedByUser"));

            bool isOpen = isOpenRecord != null && Convert.ToBoolean(isOpenRecord.GetValueByKey("_value"));
            bool isOpendByUser = isOpendByUserRecord != null && Convert.ToBoolean(isOpendByUserRecord.GetValueByKey("_value"));
            bool isPublic = isPublicRecord != null && Convert.ToBoolean(isPublicRecord.GetValueByKey("_value"));
            bool isEntering = isEnteringRecord != null && Convert.ToBoolean(isEnteringRecord.GetValueByKey("_value"));
            string licencePlate = rows[0].GetValueByKey("licencePlate") != null ? rows[0].GetValueByKey("licencePlate").ToString() : "";
            string actionBy = rows[0].GetValueByKey("actionBy") != null ? rows[0].GetValueByKey("actionBy").ToString() : "";
            

            return new VehicleGateData
            {
                Timestamp = timestamp,
                IsOpen = isOpen,
                IsPublic = isPublic,
                IsEntering = isEntering,
                LicencePlate = licencePlate,
                ActionBy = actionBy,
                IsOpenedByUser = isOpendByUser
            };
        }
    }
}
