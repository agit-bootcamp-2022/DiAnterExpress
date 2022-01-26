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

        public Task<IEnumerable<Shipment>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<Shipment> GetById(int id)
        {
            throw new NotImplementedException();
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

        public Task<Shipment> Update(int id, Shipment obj)
        {
            throw new NotImplementedException();
        }
    }
}