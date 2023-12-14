using Data.Models.SPU;
using IntelliHome_Backend.Features.SPU.DataRepositories;
using IntelliHome_Backend.Features.SPU.DataRepositories.Interfaces;
using IntelliHome_Backend.Features.SPU.DTOs;
using IntelliHome_Backend.Features.SPU.Handlers.Interfaces;
using IntelliHome_Backend.Features.SPU.Repositories.Interfaces;
using IntelliHome_Backend.Features.SPU.Services.Interfaces;

namespace IntelliHome_Backend.Features.SPU.Services
{
    public class VehicleGateService : IVehicleGateService
    {
        private readonly IVehicleGateRepository _vehicleGateRepository;
        private readonly IVehicleGateHandler _vehicleGateHandler;
        private readonly IVehicleGateDataRepository _vehicleGateDataRepository;

        public VehicleGateService(IVehicleGateRepository vehicleGateRepository, IVehicleGateHandler vehicleGateHandler, IVehicleGateDataRepository vehicleGateDataRepository)
        {
            _vehicleGateRepository = vehicleGateRepository;
            _vehicleGateHandler = vehicleGateHandler;
            _vehicleGateDataRepository = vehicleGateDataRepository;
        }

        public async Task<VehicleGate> Create(VehicleGate entity)
        {
            entity = await _vehicleGateRepository.Create(entity);
            Dictionary<string, object> additionalAttributes = new Dictionary<string, object>
                        {
                            { "is_public", entity.IsPublic },
                            { "allowed_licence_plates", entity.AllowedLicencePlates },
                            { "power_per_hour", entity.PowerPerHour }
                        };
            bool success = await _vehicleGateHandler.ConnectToSmartDevice(entity, additionalAttributes);
            if (success)
            {
                entity.IsConnected = true;
                await _vehicleGateRepository.Update(entity);
            }
            return entity;
        }

        

        public async Task<VehicleGateDTO> GetWithData(Guid id)
        {
            VehicleGate vehicleGate = await _vehicleGateRepository.Read(id);
            VehicleGateDTO vehicleGateDTO = new()
            {
                Id = vehicleGate.Id,
                Name = vehicleGate.Name,
                IsConnected = vehicleGate.IsConnected,
                IsOn = vehicleGate.IsOn,
                Category = vehicleGate.Category.ToString(),
                Type = vehicleGate.Type.ToString(),
                PowerPerHour = vehicleGate.PowerPerHour,
                IsPublic = vehicleGate.IsPublic,
                AllowedLicencePlates = vehicleGate.AllowedLicencePlates,
            };

            VehicleGateData vehicleGateData = null;
            try
            {
                vehicleGateData = GetLastData(id);
            }
            catch (Exception)
            {
                vehicleGateData = null;
            }

            if (vehicleGateData == null) return vehicleGateDTO;

            vehicleGateDTO.CurrentLicencePlate = vehicleGateData.LicencePlate;
            vehicleGateDTO.IsEntering = vehicleGateData.IsEntering;
            vehicleGateDTO.IsOpen = vehicleGateData.IsOpen;
            vehicleGateDTO.IsOpenByUser = vehicleGateData.IsOpenByUser;
            vehicleGateDTO.ActionBy = vehicleGateData.ActionBy;

            return vehicleGateDTO;
        }

        private VehicleGateData GetLastData(Guid id)
        {
            return _vehicleGateDataRepository.GetLastData(id);
        }

        public List<VehicleGateData> GetHistoricalData(Guid id, DateTime from, DateTime to)
        {
            return _vehicleGateDataRepository.GetHistoricalData(id, from, to);
        }

        public void AddPoint(Dictionary<string, object> fields, Dictionary<string, string> tags)
        {
            _vehicleGateDataRepository.AddPoint(fields, tags);
        }

        public async Task AddLicencePlate(Guid id, string licencePlate)
        {
            VehicleGate vehicleGate = await _vehicleGateRepository.FindWithSmartHome(id);
            vehicleGate.AllowedLicencePlates.Add(licencePlate);
            await _vehicleGateRepository.Update(vehicleGate);

            _vehicleGateHandler.AddLicencePlate(vehicleGate, licencePlate);
        }

        public async Task RemoveLicencePlate(Guid id, string licencePlate)
        {
            VehicleGate vehicleGate = await _vehicleGateRepository.FindWithSmartHome(id);
            vehicleGate.AllowedLicencePlates.Remove(licencePlate);
            await _vehicleGateRepository.Update(vehicleGate);

            _vehicleGateHandler.RemoveLicencePlate(vehicleGate, licencePlate);
        }

        public async Task OpenCloseGate(Guid id, bool isOpen, string username)
        {
            VehicleGate vehicleGate = await _vehicleGateRepository.FindWithSmartHome(id);
            _vehicleGateHandler.OpenCloseGate(vehicleGate, isOpen, username);
        }

        public async Task ChangeMode(Guid id, bool isPublic)
        {
            VehicleGate vehicleGate = await _vehicleGateRepository.FindWithSmartHome(id);
            _vehicleGateHandler.ChangeMode(vehicleGate, isPublic);
            vehicleGate.IsPublic = isPublic;
            await _vehicleGateRepository.Update(vehicleGate);
        }

      

        public async Task TurnOnSmartDevice(Guid id, bool turnOn)
        {
            VehicleGate vehicleGate = await _vehicleGateRepository.FindWithSmartHome(id);
            await _vehicleGateHandler.ToggleSmartDevice(vehicleGate, turnOn);

            vehicleGate.IsOn = turnOn;
            await _vehicleGateRepository.Update(vehicleGate);
        }


        public Task<VehicleGate> Delete(Guid id)
        {
            return _vehicleGateRepository.Delete(id);
        }

        public Task<VehicleGate> Get(Guid id)
        {
            return _vehicleGateRepository.Read(id);
        }

        public Task<IEnumerable<VehicleGate>> GetAll()
        {
            return _vehicleGateRepository.ReadAll();
        }

        public Task<VehicleGate> Update(VehicleGate entity)
        {
            return _vehicleGateRepository.Update(entity);
        }

        
    }
}
