using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiAnterExpress.Models;
using Microsoft.EntityFrameworkCore;

namespace DiAnterExpress.Data
{
    public class DALBranch : IBranch
    {
        private ApplicationDbContext _db;
        public DALBranch(ApplicationDbContext db)
        {
            _db = db;
        }

        public Task<Branch> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteById (int id)
        {
            var result = await GetById(id);
            if (result == null) throw new Exception("Data tidak ditemukan!");
            try
            {
                _db.Branches.Remove(result);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception($"Error: {dbEx.Message}");
            }
        }

        public async Task<IEnumerable<Branch>> GetAll()
        {
            var results = await _db.Branches.OrderBy(b => b.Name).ToListAsync();
            return results;
        }

        public async Task<Branch> GetById(int id)
        {
            var result = await _db.Branches.Where(s => s.Id == id).SingleOrDefaultAsync<Branch>();
            if (result != null)
                return result;
            else
                throw new Exception("Data tidak ditemukan !");
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
                await _db.SaveChangesAsync();
                obj.Id = id;
                return obj;
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception($"Error: {dbEx.Message}");
            }
        }
    }
}