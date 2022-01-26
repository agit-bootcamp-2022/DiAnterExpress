using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiAnterExpress.Models;
using Microsoft.EntityFrameworkCore;

namespace DiAnterExpress.Data
{
    public class DALTransactionInternal : ITransactionInternal
    {
        private ApplicationDbContext _db;

        public DALTransactionInternal(ApplicationDbContext db)
        {
            _db = db;
        }

        public Task<TransactionInternal> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TransactionInternal>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<TransactionInternal> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<TransactionInternal> Insert(TransactionInternal obj)
        {
            try
            {
                _db.TransactionInternals.Add(obj);
                await _db.SaveChangesAsync();
                return obj;
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception($"Error: {dbEx.Message}");
            }
        }

        public Task<TransactionInternal> Update(int id, TransactionInternal obj)
        {
            throw new NotImplementedException();
        }
    }
}