using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiAnterExpress.Dtos;
using DiAnterExpress.Models;
using Microsoft.EntityFrameworkCore;

namespace DiAnterExpress.Data
{
    public class DALShipmentType : IShipmentType
    {

        private ApplicationDbContext _db;

        public DALShipmentType(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<ShipmentType> Delete(int id)
        {
            try
            {
                var data = await GetById(id);

                _db.ShipmentTypes.Remove(data);
                await _db.SaveChangesAsync();

                return data;
            }
            catch (System.Exception)
            {
                throw;
            }

        }

        public async Task<IEnumerable<ShipmentType>> GetAll()
        {
            var shipmentTypes = await _db.ShipmentTypes.ToListAsync();
            return shipmentTypes;
        }

        public async Task<ShipmentType> GetById(int id)
        {
            var shipmentType = await _db.ShipmentTypes.Where(
                x => x.Id == id
            ).FirstOrDefaultAsync();

            if (shipmentType == null) throw new Exception("ShipmentType not found");

            return shipmentType;
        }

        public async Task<ShipmentType> Insert(ShipmentType obj)
        {
            try
            {
                var newShipmentTypes = new ShipmentType
                {
                    Name = obj.Name,
                    CostPerKg = obj.CostPerKg,
                    CostPerKm = obj.CostPerKm,
                };

                _db.ShipmentTypes.Add(newShipmentTypes);
                await _db.SaveChangesAsync();

                return newShipmentTypes;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task<ShipmentType> Update(int id, ShipmentType obj)
        {
            try
            {
                var shipmentType = await GetById(id);

                shipmentType.Name = obj.Name;
                shipmentType.CostPerKg = obj.CostPerKg;
                shipmentType.CostPerKm = obj.CostPerKm;

                _db.ShipmentTypes.Update(shipmentType);
                await _db.SaveChangesAsync();

                return shipmentType;
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}