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
    public class DALShipment : IShipment
    {
        private ApplicationDbContext _db;
        public DALShipment(ApplicationDbContext db)
        {
            _db = db;
        }

        public Task<Shipment> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Shipment>> GetAll()
        {
            var results = await _db.Shipments.ToListAsync();
            return results;
        }

        public async Task<Shipment> GetById(int id)
        {
            var result = await _db.Shipments.Where(s => s.Id == Convert.ToInt32(id)).SingleOrDefaultAsync<Shipment>();
            if (result != null)
                return result;
            else
                throw new Exception("Data tidak ditemukan !");
        }

        public Task<double> GetShipmentFee(ShipmentFeeInput input, double costPerKm, double costPerKg)
        {
            var senderLocation = new Point(input.SenderLong, input.SenderLat) { SRID = 4326 };
            var receiverLocation = new Point(input.ReceiverLong, input.ReceiverLat) { SRID = 4326 };
            var distance = Math.Ceiling(senderLocation.Distance(receiverLocation) / 1000);
            var fee = Math.Ceiling((((distance * costPerKm) + (input.Weight * costPerKg)) / 500)) * 500;
            return Task.FromResult(fee);
        }

        public async Task<Shipment> Insert(Shipment obj)
        {
            try
            {
                _db.Shipments.Add(obj);
                await _db.SaveChangesAsync();
                return obj;
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception($"Error: {dbEx.Message}");
            }
        }

        public async Task<Shipment> Update(int id, Shipment obj)
        {
            try
            {
                var oldShipment = await GetById(id);

                oldShipment.Status = obj.Status;

                await _db.SaveChangesAsync();
                return oldShipment;
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}