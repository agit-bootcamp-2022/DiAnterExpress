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
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace DiAnterExpress.SyncDataServices.Http
{
    public class HttpShipmentInternalDataClient : IShipmentInternalDataClient
    {
        private readonly GraphQLHttpClient _client;
        public HttpShipmentInternalDataClient(GraphQLHttpClient client)
        {
            _client = client;
        }
        public async Task<TransactionStatus> CreateShipmentInternal(TransferBalanceDto transferBalanceDto, string token)
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
            var response = await _client.SendMutationAsync<object>(query);
            var stringResult = response.Data.ToString();
            stringResult = stringResult.Replace($"\"transferBalance\":", string.Empty);
            stringResult = stringResult.Remove(0, 1);
            stringResult = stringResult.Remove(stringResult.Length - 1, 1);
            var result = JsonConvert.DeserializeObject<TransactionStatus>(stringResult);
            return result;
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
                var response = await _client.SendMutationAsync<object>(query);
                var stringResult = response.Data.ToString();
                stringResult = stringResult.Replace($"\"loginUser\":", string.Empty);
                stringResult = stringResult.Remove(0, 1);
                stringResult = stringResult.Remove(stringResult.Length - 1, 1);

                var result = JsonConvert.DeserializeObject<UserToken>(stringResult);
                return result;
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
                var response = await _client.SendQueryAsync<object>(query);
                var stringResult = response.Data.ToString();
                stringResult = stringResult.Replace($"\"profileByCustomerIdAsync\": [", string.Empty);
                stringResult = stringResult.Replace($"]", string.Empty);
                stringResult = stringResult.Remove(0, 1);
                stringResult = stringResult.Remove(stringResult.Length - 1, 1);

                var result = JsonConvert.DeserializeObject<ProfileOutput>(stringResult);
                return result;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}
