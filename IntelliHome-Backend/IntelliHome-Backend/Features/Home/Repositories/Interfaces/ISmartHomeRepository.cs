﻿using Data.Models.Home;
using Data.Models.Users;
using IntelliHome_Backend.Features.Shared.Repositories.Interfaces;

namespace IntelliHome_Backend.Features.Home.Repositories.Interfaces
{
    public interface ISmartHomeRepository : ICrudRepository<SmartHome>
    {
        Task<List<SmartHome>> GetSmartHomesForUser(User user);
    }
}
