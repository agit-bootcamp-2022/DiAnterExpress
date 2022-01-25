using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiAnterExpress.Models;

namespace DiAnterExpress.External.SyncDataService.Http
{
    public interface IShipmentFeeClient
    {
        Task<ShipmentType> GetFee(int id);
    }
}