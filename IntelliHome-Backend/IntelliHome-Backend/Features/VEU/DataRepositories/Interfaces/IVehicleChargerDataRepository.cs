﻿using IntelliHome_Backend.Features.Shared.DTOs;
using IntelliHome_Backend.Features.VEU.DTOs.BatterySystem;
using IntelliHome_Backend.Features.VEU.DTOs.VehicleCharger;

namespace IntelliHome_Backend.Features.VEU.DataRepositories.Interfaces
{
    public interface IVehicleChargerDataRepository
    {
        void AddActionMeasurement(Dictionary<string, object> fields, Dictionary<string, string> tags);
        List<ActionDataDTO> GetActionHistoricalData(Guid id, DateTime from, DateTime to);
    }
}
