using Data.Models.PKA;
using Data.Models.SPU;
using IntelliHome_Backend.Features.Shared.Services.Interfaces;
using IntelliHome_Backend.Features.SPU.DTOs;

namespace IntelliHome_Backend.Features.SPU.Services.Interfaces
{
    public interface ILampService : ICrudService<Lamp>
    {
        Task<LampDTO> GetWithData(Guid id);
        List<LampData> GetHistoricalData(Guid id, DateTime from, DateTime to);
        void AddPoint(Dictionary<string, object> fields, Dictionary<string, string> tags);
        Task ChangeMode(Guid id, bool isAuto);
        Task ChangeBrightnessLimit(Guid id, double brightness);
        Task TurnOnSmartDevice(Guid id, bool turnOn);
        Task TurnLampOn(Guid id, Boolean isOn);
    }
}
