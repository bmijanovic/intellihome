﻿using Data.Models.PKA;
using IntelliHome_Backend.Features.PKA.DTOs;
using IntelliHome_Backend.Features.Shared.Services.Interfaces;

namespace IntelliHome_Backend.Features.PKA.Services.Interfaces
{
    public interface IAmbientSensorService : ICrudService<AmbientSensor>
    {
        IEnumerable<AmbientSensor> GetAllWithHome();
        List<AmbientSensorData> GetHistoricalData(Guid id, DateTime from, DateTime to);
        void AddPoint(Dictionary<string, object> fields, Dictionary<string, string> tags);
        Task<AmbientSensorDTO> GetWithData(Guid id);
        List<AmbientSensorData> GetLastHourData(Guid id);
        Task ToggleAmbientSensor(Guid id, bool turnOn = true);
    }
}
