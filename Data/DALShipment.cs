using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiAnterExpress.Models;

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