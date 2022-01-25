using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiAnterExpress.Models;

namespace DiAnterExpress.Data
{
    public class DALBranch : IBranch
    {
        public DALBranch()
        {

        }

        public Task<Branch> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Branch>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<Branch> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Branch> Insert(Branch obj)
        {
            throw new NotImplementedException();
        }

        public Task<Branch> Update(int id, Branch obj)
        {
            throw new NotImplementedException();
        }
    }
}