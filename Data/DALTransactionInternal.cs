using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiAnterExpress.Models;

namespace DiAnterExpress.Data
{
    public class DALTransactionInternal : ITransactionInternal
    {
        public DALTransactionInternal()
        {

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

        public Task<TransactionInternal> Insert(TransactionInternal obj)
        {
            throw new NotImplementedException();
        }

        public Task<TransactionInternal> Update(int id, TransactionInternal obj)
        {
            throw new NotImplementedException();
        }
    }
}