using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using ParkyAPI.Data;
using ParkyAPI.Models;
using ParkyAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.Repository
{
    public class NationalParkRepository : INationalParkRepository
    {
        private readonly ParkyAPIdbContext _dbContext;


        public NationalParkRepository(ParkyAPIdbContext dbContext)
        {
            this._dbContext = dbContext;
        }



        public bool CreateNationalPark(NationalPark park)
        {
            this._dbContext.NationalParks.Add(park);
            return Save();
        }

        public bool DeleteNationalPark(NationalPark park)
        {
            this._dbContext.NationalParks.Remove(park);
            return Save();
        }

        public NationalPark GetNationalPark(int nationalParkID)
        {
            return this._dbContext.NationalParks.FirstOrDefault( park =>
                    park.Id == nationalParkID);
        }

        public ICollection<NationalPark> GetNationalParks()
        {
            return this._dbContext.NationalParks.OrderBy(park =>
                   park.Name).ToList();
        }

        public bool NationalParkExists(string name)
        {
            bool value = this._dbContext.NationalParks.Any(park =>
                          park.Name.ToLower().Trim() == name.ToLower().Trim());

            return value;
        }

        public bool NationalParkExists(int id)
        {
            return this._dbContext.NationalParks.Any(park => park.Id == id);
        }

        public bool UpdateNationalPark(NationalPark park)
        {
            this._dbContext.NationalParks.Update(park);
            return Save();
        }

        public bool Save()
        {
            return this._dbContext.SaveChanges() >= 0 ? true : false;
        }

        
    }
}
