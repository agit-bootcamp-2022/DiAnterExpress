using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiAnterExpress.Dtos;
using DiAnterExpress.Models;
using NetTopologySuite.Geometries;

namespace DiAnterExpress.Data
{
    public class DALShipment : IShipment
    {
        public DALShipment()
        {

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

        public Task<Shipment> Insert(Shipment obj)
        {
            throw new NotImplementedException();
        }

        public Task<Shipment> Update(int id, Shipment obj)
        {
            throw new NotImplementedException();
        }
    }
}