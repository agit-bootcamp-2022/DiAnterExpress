using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiAnterExpress.Dtos;

namespace DiAnterExpress.SyncDataServices.Http
{
    public interface IShipmentInternalDataClient
    {
        Task<TransferBalanceDto> CreateShipmentInternal(TransferBalanceDto transferBalanceDto);
    }
}