using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiAnterExpress.Dtos;
using DiAnterExpress.Models;

namespace DiAnterExpress.Externals
{
    public interface ITokopodiaDataClient
    {
        Task ShipmentDelivered(int id, string token);
    }
}