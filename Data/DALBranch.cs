using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiAnterExpress.Dtos;
using DiAnterExpress.Models;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace DiAnterExpress.Data
{
    public class DALBranch : IBranch
    {
        private ApplicationDbContext _db;
        public DALBranch(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Branch> Delete(int id)
        {
            try
            {
                var result = await GetById(id);

                if (result == null) throw new Exception("Data tidak ditemukan!");

                _db.Branches.Remove(result);
                await _db.SaveChangesAsync();

                return result;
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception($"Error: {dbEx.Message}");
            }
        }

        public async Task<IEnumerable<Branch>> GetAll()
        {
            var results = await _db.Branches.ToListAsync();
            return results;
        }

        public async Task<Branch> GetById(int id)
        {
            var result = await _db.Branches.Where(
                branch => branch.Id == id
            ).SingleOrDefaultAsync<Branch>();

            if (result == null) throw new Exception("Branch tidak ditemukan!");

            return result;
        }

        public async Task<Branch> GetNearestByLocation(Dtos.Location location)
        {
            try
            {
                var locationPoint = new Point(location.Longitude, location.Latitude) { SRID = 4326 };

                var nearestBranch = await _db.Branches.OrderBy(
                    b => b.Location.Distance(locationPoint)
                ).FirstOrDefaultAsync();

                if (nearestBranch == null) throw new Exception("Branch not found");

                return nearestBranch;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task<Branch> GetByUserId(string id)
        {
            var result = await _db.Branches.Where(
                branch => branch.UserId == id
            ).FirstOrDefaultAsync();

            if (result == null) throw new Exception("UserId not found");

            return result;
        }

        public async Task<Branch> Insert(Branch obj)
        {
            try
            {
                _db.Branches.Add(obj);
                await _db.SaveChangesAsync();
                return obj;
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception($"Error: {dbEx.Message}");
            }
        }

        public async Task<Branch> Update(int id, Branch obj)
        {
            try
            {
                var result = await GetById(id);

                result.Name = obj.Name;
                result.Address = obj.Address;
                result.City = obj.City;
                result.Phone = obj.Phone;

                _db.Branches.Update(result);
                await _db.SaveChangesAsync();

                return result;
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception($"Error: {dbEx.Message}");
            }
        }
    }
}