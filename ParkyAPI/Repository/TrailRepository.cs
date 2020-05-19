using Microsoft.EntityFrameworkCore;
using ParkyAPI.Data;
using ParkyAPI.Models;
using ParkyAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.Repository
{
    public class TrailRepository : ITrailRepository
    {
        private readonly ParkyAPIdbContext _dbContext;


        public TrailRepository(ParkyAPIdbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public ICollection<Trail> GetTrailsInNationalPark(int nationalParkID)
        {
            return this._dbContext.Trails.Include(trail => trail.NationalPark)
                                  .Where(trail => trail.NationalParkId == nationalParkID)
                                  .ToList(); 
        }

        public bool CreateTrail(Trail trail)
        {
            this._dbContext.Trails.Add(trail);
            return Save();
        }

        public bool DeleteTrail(Trail trail)
        {
            this._dbContext.Trails.Remove(trail);
            return Save();
        }

        public Trail GetTrail(int trailID)
        {
            return this._dbContext.Trails.Include( trail => trail.NationalPark)
                                  .FirstOrDefault(trail => trail.Id == trailID);
        }

        public ICollection<Trail> GetTrails()
        {
            var x = this._dbContext.Trails.Include(trail => trail.NationalPark)
                                         .OrderBy(trail => trail.Name).ToList();

            return this._dbContext.Trails.Include(trail => trail.NationalPark)
                                         .OrderBy(trail => trail.Name).ToList();
        }

        public bool TrailExists(string name)
        {
            bool value = this._dbContext.Trails.Any(trail =>
                          trail.Name.ToLower().Trim() == name.ToLower().Trim());

            return value;
        }

        public bool TrailExists(int id)
        {
            return this._dbContext.Trails.Any(trail => trail.Id == id);
        }

        public bool UpdateTrail(Trail trail)
        {
            this._dbContext.Trails.Update(trail);
            return Save();
        }

        public bool Save()
        {
            return this._dbContext.SaveChanges() >= 0 ? true : false;
        }
    }
}
