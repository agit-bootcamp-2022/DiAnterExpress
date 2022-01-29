using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiAnterExpress.Dtos;
using DiAnterExpress.Models;

namespace DiAnterExpress.Data
{
    public interface IBranch : ICrud<Branch>
    {
        Task DeleteById(int id);
        Task<Branch> GetNearestByLocation(Location location);
        Task<Branch> GetByUserId(string id);
    }
}