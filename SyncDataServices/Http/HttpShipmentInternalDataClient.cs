using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DiAnterExpress.Dtos;
using DiAnterExpress.Models;
using GraphQL;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace DiAnterExpress.SyncDataServices.Http
{
    public class HttpShipmentInternalDataClient : IShipmentInternalDataClient
    {
        private readonly GraphQLHttpClient _client;
        private readonly IConfiguration _configuration;

        public HttpShipmentInternalDataClient(IConfiguration configuration)
        {
            _configuration = configuration;

            _client = new GraphQLHttpClient(
                _configuration["UangTransURI"],
                new NewtonsoftJsonSerializer()
            );
        }
        public async Task<TransferBalanceOutput> CreateShipmentInternal(TransferBalanceDto transferBalanceDto, string token)
        {
            try
            {
                var query = new GraphQLRequest
                {
                    Query = @"
                        mutation ($input: TransferBalanceInput!) {
                            transferBalance(
                                input: $input
                            ) {
                            succeed
                            message
                            }
                        }
                        ",
                    Variables = new { input = transferBalanceDto }
                };

                _client.HttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

                var result = await _client.SendMutationAsync<ReturnData>(query);
                if (result.Data.TransferBalance == null) throw new Exception("Transfer Balance Failed");

                return result.Data.TransferBalance;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }

        }

        public async Task<UserToken> LoginUser(LoginUserInput userInput)
        {
            try
            {
                var query = new GraphQLRequest
                {
                    Query = @"
                            mutation ($input: LoginUserInput!) {
                                loginUser(input: $input) {
                                    token
                                    expired
                                    message
                                }
                            }
                            ",
                    Variables = new { input = userInput }
                };

                var result = await _client.SendMutationAsync<ReturnData>(query);
                if (result.Data.LoginUser == null) throw new Exception("Login to UangTrans failed");

                return result.Data.LoginUser;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        public async Task<ProfileOutput> GetProfile(string token)
        {
            try
            {
                var query = new GraphQLRequest
                {
                    Query = @"
                            query {
                                profileByCustomerIdAsync {
                                id 
                                username 
                                firstName 
                                lastName 
                                email 
                                createdDate
                                }
                            }"
                };

                _client.HttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

                var result = await _client.SendQueryAsync<ReturnData>(query);
                if (result.Data.ProfileByCustomerIdAsync == null) throw new Exception("Failed to fetch UangTrans User Profile");

                return result.Data.ProfileByCustomerIdAsync[0];
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        public async Task<TransactionStatus> ShipmentDelivered(int id, string token)
        {
            try
            {
                var query = new GraphQLRequest
                {
                    Query =
                    @"
                    mutation {
                        updateStatusTransaction( 
                            input:
                            {
                                transactionId: $transactionId
                            }
                        )
                        {
                            succeed, message
                        }
                    }
                ",
                    Variables = new { transactionId = id },
                };

                _client.HttpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var response = await _client.SendMutationAsync<ReturnData>(query);
                if (!response.Data.TransactionStatus.Succeed || response.Data.TransactionStatus == null)
                    throw new Exception("Failed to update transaction status to UangTrans");

                return response.Data.TransactionStatus;
            }
            catch (System.Exception)
            {
                throw;
            }

        }
    }
}
