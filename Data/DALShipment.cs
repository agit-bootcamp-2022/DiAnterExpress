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
        
        Task<Shipment> ICrud<Shipment>.Delete(int id)
        {
            throw new NotImplementedException();
        }

        Task<Shipment> ICrud<Shipment>.Insert(Shipment obj)
        {
            throw new NotImplementedException();
        }

        Task<Shipment> ICrud<Shipment>.Update(int id, Shipment obj)
        {
            throw new NotImplementedException();
        }
    }
}