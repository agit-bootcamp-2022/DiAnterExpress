using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiAnterExpress.Dtos;
using DiAnterExpress.Models;
using NetTopologySuite.Geometries;
using Microsoft.EntityFrameworkCore;

namespace DiAnterExpress.Data
{
    public class DALShipment : IShipment
    {
        private ApplicationDbContext _db;
        public DALShipment(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Shipment> Delete(int id)
        {
            var result = await GetById(id);
            if (result == null) throw new Exception("Data tidak ditemukan!");
            try
            {
                _db.Shipments.Remove(result);
                await _db.SaveChangesAsync();
                return result;
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception($"Error: {dbEx.Message}");
            }
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
            var senderLocation = new Point(input.SenderAddress.Latitude, input.SenderAddress.Longitude) { SRID = 4326 };
            var receiverLocation = new Point(input.ReceiverAddress.Latitude, input.ReceiverAddress.Longitude) { SRID = 4326 };
            var distance = senderLocation.Distance(receiverLocation) / 1000;
            var fee = (distance * costPerKm) + (input.Weight * costPerKg);
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
                var result = await GetById(id);
                result.SenderName = obj.SenderName;
                result.SenderContact = obj.SenderContact;
                result.SenderAddress = obj.SenderAddress;
                result.ReceiverName = obj.ReceiverName;
                result.ReceiverContact = obj.ReceiverContact;
                result.ReceiverAddress = obj.ReceiverAddress;
                result.TotalWeight = obj.TotalWeight;
                result.Cost = obj.Cost;
                result.SenderAddress = obj.SenderAddress;
                await _db.SaveChangesAsync();
                obj.Id = Convert.ToInt32(id);
                return obj;
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception($"Error: {dbEx.Message}");
            }
        }
    }
}