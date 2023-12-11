﻿using InfluxDB.Client.Core.Flux.Domain;
using IntelliHome_Backend.Features.Shared.Influx;
using IntelliHome_Backend.Features.VEU.DataRepositories.Interfaces;
using IntelliHome_Backend.Features.VEU.DTOs;

namespace IntelliHome_Backend.Features.VEU.DataRepositories
{
    public class BatterySystemDataRepository : IBatterySystemDataRepository
    {
        private readonly InfluxRepository _context;

        public BatterySystemDataRepository(InfluxRepository context)
        {
            _context = context;
        }

        public List<BatterySystemDataDTO> GetHistoricalData(Guid id, DateTime from, DateTime to)
        {
            var result = _context.GetHistoricalData(id, from, to).Result;
            return result.Select(ConvertToBatterySystemDataDTO).ToList();
        }


        public void AddPoint(Dictionary<string, object> fields, Dictionary<string, string> tags)
        {
            _context.WriteToInfluxAsync("battery_system", fields, tags);
        }

        public BatterySystemDataDTO GetLastData(Guid id)
        {
            var table = _context.GetLastData(id).Result;
            return ConvertToBatterySystemDataDTO(table);
        }

        private BatterySystemDataDTO ConvertToBatterySystemDataDTO(FluxTable table)
        {
            var rows = table.Records;
            DateTime timestamp = DateTime.Parse(rows[0].GetValueByKey("_time").ToString());
            TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
            timestamp = TimeZoneInfo.ConvertTime(timestamp, localTimeZone);

            var currentCapacityRecord = rows.FirstOrDefault(r => r.Row.Contains("current_capacity"));
            double currentCapacity = currentCapacityRecord != null ? Convert.ToDouble(currentCapacityRecord.GetValueByKey("_value")) : 0.0;

            var consumptionPerMinuteRecord = rows.FirstOrDefault(r => r.Row.Contains("consumption_per_minute"));
            double consumptionPerMinut = consumptionPerMinuteRecord != null ? Convert.ToDouble(consumptionPerMinuteRecord.GetValueByKey("_value")) : 0.0;

            var gridPerMinuteRecord = rows.FirstOrDefault(r => r.Row.Contains("grid_per_minute"));
            double gridPerMinute = gridPerMinuteRecord != null ? Convert.ToDouble(gridPerMinuteRecord.GetValueByKey("_value")) : 0.0;

            return new BatterySystemDataDTO
            {
                Timestamp = timestamp,
                CurrentCapacity = currentCapacity,
                ConsumptionPerMinute = consumptionPerMinut,
                GridPerMinute = gridPerMinute
            };
        }
    }
}