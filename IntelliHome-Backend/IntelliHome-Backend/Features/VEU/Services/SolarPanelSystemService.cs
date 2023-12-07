﻿using Data.Models.VEU;
using IntelliHome_Backend.Features.Shared.Exceptions;
using IntelliHome_Backend.Features.VEU.Handlers;
using IntelliHome_Backend.Features.VEU.Handlers.Interfaces;
using IntelliHome_Backend.Features.VEU.Repositories;
using IntelliHome_Backend.Features.VEU.Repositories.Interfaces;
using IntelliHome_Backend.Features.VEU.Services.Interfaces;

namespace IntelliHome_Backend.Features.VEU.Services
{
    public class SolarPanelSystemService : ISolarPanelSystemService
    {
        private readonly ISolarPanelSystemRepository _solarPanelSystemRepository;
        private readonly ISolarPanelSystemHandler _solarPanelSystemHandler;

        public SolarPanelSystemService(ISolarPanelSystemRepository solarPanelSystemRepository, ISolarPanelSystemHandler solarPanelSystemHandler)
        {
            _solarPanelSystemRepository = solarPanelSystemRepository;
            _solarPanelSystemHandler = solarPanelSystemHandler;
        }

        public async Task<SolarPanelSystem> Create(SolarPanelSystem entity)
        {
            entity = await _solarPanelSystemRepository.Create(entity);
            Dictionary<string, object> additionalAttributes = new Dictionary<string, object>
            {
                { "area", entity.Area },
                { "efficiency", entity.Efficiency }
            };
            bool success = await _solarPanelSystemHandler.ConnectToSmartDevice(entity, additionalAttributes);
            if (success)
            {
                entity.IsConnected = true;
                await _solarPanelSystemRepository.Update(entity);
            }
            return entity;
        }

        public Task<SolarPanelSystem> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<SolarPanelSystem> Get(Guid id)
        {
            SolarPanelSystem solarPanelSystem = await _solarPanelSystemRepository.Read(id);
            if (solarPanelSystem == null)
            {
                throw new ResourceNotFoundException("Solar panel system with provided Id not found!");
            }
            return solarPanelSystem;
        }

        public Task<IEnumerable<SolarPanelSystem>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<SolarPanelSystem> Update(SolarPanelSystem entity)
        {
            throw new NotImplementedException();
        }
    }
}
