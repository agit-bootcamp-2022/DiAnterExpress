using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiAnterExpress.Models;

namespace DiAnterExpress.Data
{
    public class DALShipmentType : IShipmentType
    {
        private readonly ApplicationDbContext _context;

        public DALShipmentType(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<ShipmentType> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ShipmentType>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<ShipmentType> GetById(int id)
        {
            var result = _context.ShipmentTypes.Where(o => o.Id == id).FirstOrDefault();
            return Task.FromResult(result);
        }

        public Task<ShipmentType> Insert(ShipmentType obj)
        {
            throw new NotImplementedException();
        }

        public Task<ShipmentType> Update(int id, ShipmentType obj)
        {
            throw new NotImplementedException();
        }
    }
}