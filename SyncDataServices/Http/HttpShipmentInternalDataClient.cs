using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DiAnterExpress.Dtos;
using DiAnterExpress.Models;
using GraphQL;
using GraphQL.Client.Abstractions;
using Microsoft.Extensions.Configuration;

namespace DiAnterExpress.SyncDataServices.Http
{
    public class HttpShipmentInternalDataClient : IShipmentInternalDataClient
    {
        private readonly IGraphQLClient _client;
        private readonly IConfiguration _configuration;

        public HttpShipmentInternalDataClient(IGraphQLClient client)
        {
            _client = client;
        }
        public async Task<TransactionStatus> CreateShipmentInternal(TransferBalanceDto transferBalanceDto)
        {
            var query = new GraphQLRequest
            {
                Query = @"
                        mutation TransferBalance{
                            transferBalance(input: $input{
                                customerDebitId,
                                amount,
                                customerCreditId
                            }) {
                                succeed, message
                            }
                        }",
                Variables = new{input = transferBalanceDto},
            };
            var response = await _client.SendMutationAsync<GetTransferBalanceDto>(query);
            return response.Data.transactionStatus;
        }
    }
}
