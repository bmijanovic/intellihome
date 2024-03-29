﻿using Data.Context;
using Data.Models.Home;
using IntelliHome_Backend.Features.Home.Repositories.Interfaces;
using IntelliHome_Backend.Features.Shared.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IntelliHome_Backend.Features.Home.Repositories
{
    public class CityRepository : CrudRepository<City>, ICityRepository
    {
        public CityRepository(PostgreSqlDbContext context) : base(context)
        {
        }

        public async Task<City> FindByNameAndCountry(string city, string country)
        {
            return _entities.FirstOrDefault(c => c.Name == city && c.Country == country);
        }

        public async Task<List<City>> GetCitiesWithNameSearch(string search) 
        {
            return await _entities.Where(s => s.Name.ToLower().Contains(search.ToLower())).ToListAsync();
        }
    }
}
