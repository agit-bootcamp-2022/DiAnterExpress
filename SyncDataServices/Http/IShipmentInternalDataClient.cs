using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiAnterExpress.Dtos;

namespace DiAnterExpress.SyncDataServices.Http
{
    public interface IShipmentInternalDataClient
    {
        Task<TransferBalanceOutput> CreateShipmentInternal(TransferBalanceDto transferBalanceDto, string token);
        Task<UserToken> LoginUser(LoginUserInput userInput);
        Task<ProfileOutput> GetProfile(string token);
        Task<TransactionStatus> UpdateStatusTransaction(int id, string token);
    }
}