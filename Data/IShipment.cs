using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiAnterExpress.Dtos;
using DiAnterExpress.Models;

namespace DiAnterExpress.Data
{
    public interface IShipment : ICrud<Shipment>
    {
        public Task<double> GetShipmentFee(ShipmentFeeInput input, double costPerKm, double costPerKg);
    }
}