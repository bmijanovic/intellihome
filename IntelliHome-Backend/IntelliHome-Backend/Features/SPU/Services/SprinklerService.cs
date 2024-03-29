﻿using Data.Models.Shared;
using Data.Models.SPU;
using IntelliHome_Backend.Features.Home.DataRepository.Interfaces;
using IntelliHome_Backend.Features.PKA.DTOs;
using IntelliHome_Backend.Features.Shared.DTOs;
using IntelliHome_Backend.Features.Shared.Exceptions;
using IntelliHome_Backend.Features.SPU.DataRepositories.Interfaces;
using IntelliHome_Backend.Features.SPU.DTOs;
using IntelliHome_Backend.Features.SPU.Handlers.Interfaces;
using IntelliHome_Backend.Features.SPU.Repositories.Interfaces;
using IntelliHome_Backend.Features.SPU.Services.Interfaces;

namespace IntelliHome_Backend.Features.SPU.Services
{
    public class SprinklerService : ISprinklerService
    {
        private readonly ISprinklerRepository _sprinklerRepository;
        private readonly ISprinklerHandler _sprinklerHandler;
        private readonly ISprinklerDataRepository _sprinklerDataRepository;
        private readonly ISprinklerWorkRepository _sprinklerWorkRepository;
        private readonly ISmartDeviceDataRepository _smartDeviceDataRepository;

        public SprinklerService(
            ISprinklerRepository sprinklerRepository, 
            ISprinklerHandler sprinklerHandler, 
            ISprinklerWorkRepository sprinklerWorkRepository,
            ISprinklerDataRepository sprinklerDataRepository, 
            ISmartDeviceDataRepository smartDeviceDataRepository)
        {
            _sprinklerRepository = sprinklerRepository;
            _sprinklerHandler = sprinklerHandler;
            _sprinklerWorkRepository = sprinklerWorkRepository;
            _sprinklerDataRepository = sprinklerDataRepository;
            _smartDeviceDataRepository = smartDeviceDataRepository;
        }

        public async Task<Sprinkler> Create(Sprinkler entity)
        {
            entity = await _sprinklerRepository.Create(entity);
            bool success = await _sprinklerHandler.ConnectToSmartDevice(entity);
            if (!success) return entity;
            entity.IsConnected = true;
            await _sprinklerRepository.Update(entity);
            var fields = new Dictionary<string, object>
            {
                { "isConnected", 1 }

            };
            var tags = new Dictionary<string, string>
            {
                { "deviceId", entity.Id.ToString()}
            };
            _smartDeviceDataRepository.AddPoint(fields, tags);
            return entity;
        }

        public async Task<Sprinkler> GetWithSmartHome(Guid id)
        {
            return await _sprinklerRepository.ReadWithSmartHome(id);
        }

        public List<SprinklerData> GetHistoricalData(Guid id, DateTime from, DateTime to)
        {
            return _sprinklerDataRepository.GetHistoricalData(id, from, to);
        }

        public async Task AddScheduledWork(string id, bool scheduleIsSpraying, string scheduleStartDate, string? scheduleEndDate,
            string username)
        {
            Sprinkler sprinkler = await _sprinklerRepository.ReadWithSmartHome(Guid.Parse(id)) ?? throw new ResourceNotFoundException("Sprinkler not found!");
            DateTime sDate = DateTime.ParseExact(scheduleStartDate, "dd/MM/yyyy HH:mm",
                System.Globalization.CultureInfo.InvariantCulture);
            if (scheduleEndDate != null)
            {
                DateTime eDate = DateTime.ParseExact(scheduleEndDate, "dd/MM/yyyy HH:mm",
                                       System.Globalization.CultureInfo.InvariantCulture);
                if (eDate < sDate)
                {
                    throw new InvalidInputException("End date must be after start date!");
                }

                SprinklerWork sprinklerWork = new SprinklerWork
                {
                    Name = "Sprinkler work",
                    IsSpraying = scheduleIsSpraying,
                    DateFrom = DateOnly.FromDateTime(sDate.Date),
                    DateTo = DateOnly.FromDateTime(eDate.Date),
                    Start = TimeOnly.FromTimeSpan(sDate.TimeOfDay),
                    End = TimeOnly.FromTimeSpan(eDate.TimeOfDay),
                    Days = new List<DaysInWeek>(),
                };
                sprinkler.ScheduledWorks.Add(sprinklerWork);
                _ = await _sprinklerRepository.Update(sprinkler);
                _sprinklerHandler.AddSchedule(sprinkler, scheduleStartDate, true);
                _sprinklerHandler.AddSchedule(sprinkler, scheduleEndDate, false);
            }
            else
            {
                SprinklerWork sprinklerWork = new()
                {
                    IsSpraying = scheduleIsSpraying,
                    DateFrom = DateOnly.FromDateTime(sDate.Date),
                    Start = TimeOnly.FromTimeSpan(sDate.TimeOfDay),
                    Name = "Sprinkler work",
                    Days = new List<DaysInWeek>(),
                };
                sprinkler.ScheduledWorks.Add(sprinklerWork);
                _ = await _sprinklerRepository.Update(sprinkler);
                _sprinklerHandler.AddSchedule(sprinkler, scheduleStartDate, true);
            }

            var fields = new Dictionary<string, object>
            {
                { "action", $"SCHEDULE MODE: ON" }

            };
            var tags = new Dictionary<string, string>
            {
                { "actionBy", username},
                { "deviceId", id}
            };
            _sprinklerDataRepository.AddActionMeasurement(fields, tags);
        }

        public async Task ToggleSprinkler(Guid id, string username, bool turnOn)
        {
            Sprinkler sprinkler = await _sprinklerRepository.ReadWithSmartHome(id) ?? throw new ResourceNotFoundException("Sprinkler not found!");
            _sprinklerHandler.ToggleSmartDevice(sprinkler, turnOn);

            sprinkler.IsOn = turnOn;
            await _sprinklerRepository.Update(sprinkler);

            var fields = new Dictionary<string, object>
            {
                { "action", turnOn ? "ON" : "OFF" }

            };
            var tags = new Dictionary<string, string>
            {
                { "actionBy", username},
                { "deviceId", id.ToString()}
            };
            _sprinklerDataRepository.AddActionMeasurement(fields, tags);
        }

        public async Task<SprinklerDTO> GetWithData(Guid id)
        {
            Sprinkler sprinkler = await _sprinklerRepository.ReadWithSmartHome(id) ?? throw new ResourceNotFoundException("Smart device not found!");
            SprinklerDTO sprinklerDTO = new SprinklerDTO
            {
                Id = sprinkler.Id,
                Name = sprinkler.Name,
                IsConnected = sprinkler.IsConnected,
                Category = sprinkler.Category.ToString(),
                Type = sprinkler.Type.ToString(),
                SmartHomeId = sprinkler.SmartHome.Id,
                PowerPerHour = sprinkler.PowerPerHour,
                IsOn = sprinkler.IsOn,
                Schedules = sprinkler.ScheduledWorks.Select(work => work.DateTo.Year != 1
                    ? new SprinklerScheduleDTO
                    {
                        Date = $"{work.DateFrom:dd/MM/yyyy} {work.Start:HH:mm} - {work.DateTo:dd/MM/yyyy} {work.End:HH:mm}",
                        IsSpraying = work.IsSpraying
                    }
                    : new SprinklerScheduleDTO
                    {
                        Date = $"{work.DateFrom:dd/MM/yyyy} {work.Start:HH:mm}",
                        IsSpraying = work.IsSpraying
                    }).ToList()
            };

            SprinklerData sprinklerData = null;
            try
            {
                sprinklerData = GetLastData(id);
            }
            catch (Exception)
            {
                sprinklerData = null;
            }

            if (sprinklerData != null)
            {
                sprinklerDTO.IsSpraying = sprinklerData.IsSpraying;
            }

            return sprinklerDTO;
        }


        public void AddPoint(Dictionary<string, object> fields, Dictionary<string, string> tags)
        {
            _sprinklerDataRepository.AddPoint(fields, tags);
        }

        public async Task ToggleSprinklerSpraying(Guid id, string username, bool turnOn)
        {
            Sprinkler sprinkler = await _sprinklerRepository.ReadWithSmartHome(id) ?? throw new ResourceNotFoundException("Sprinkler not found!");
            _sprinklerHandler.SetSpraying(sprinkler, turnOn);

            var fields = new Dictionary<string, object>
            {
                { "action", turnOn ? "SPRAYING_ON" : "SPRAYING_OFF" }

            };
            var tags = new Dictionary<string, string>
            {
                { "actionBy", username},
                { "deviceId", id.ToString()}
            };
            _sprinklerDataRepository.AddActionMeasurement(fields, tags);
        }

        private SprinklerData GetLastData(Guid id)
        {
            return _sprinklerDataRepository.GetLastData(id);
        }

        public List<ActionDataDTO> GetActionHistoricalData(Guid id, DateTime from, DateTime to)
        {
            return _sprinklerDataRepository.GetActionHistoricalData(id, from, to);
        }

        #region CRUD




        public Task<Sprinkler> Delete(Guid id)
        {
            return _sprinklerRepository.Delete(id);
        }

        public Task<Sprinkler> Get(Guid id)
        {
            return _sprinklerRepository.Read(id);
        }

        public Task<IEnumerable<Sprinkler>> GetAll()
        {
            return _sprinklerRepository.ReadAll();
        }

        public Task<Sprinkler> Update(Sprinkler entity)
        {
            return _sprinklerRepository.Update(entity);
        }
        #endregion
    }
}
