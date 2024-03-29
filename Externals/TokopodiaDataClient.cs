using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DiAnterExpress.Externals.Dtos;
using DiAnterExpress.Models;
using GraphQL;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.Extensions.Configuration;

namespace DiAnterExpress.Externals
{
    public class TokopodiaDataClient : ITokopodiaDataClient
    {
        private GraphQLHttpClient _client;
        private IConfiguration _configuration;

        public TokopodiaDataClient(IConfiguration configuration)
        {
            _configuration = configuration;

            _client = new GraphQLHttpClient(
                _configuration["TokopodiaURI"],
                new NewtonsoftJsonSerializer()
            );
        }

        public async Task TransactionUpdateStatus(int id, string token)
        {
            try
            {
                var query = new GraphQLRequest
                {
                    Query =
                    @"
                    mutation ($input: UpdateInput!){
                        updateTransaction(input: $input)
                        {
                            message
                        }
                    }
                    ",
                    Variables = new
                    {
                        input = new TransactionUpdateInputDto
                        {
                            transactionId = id
                        }
                    },
                };

                _client.HttpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var response = await _client.SendMutationAsync<TokopodiaReturnData>(query);
                if (!response.Data.UpdateTransaction.message.Equals("success")) throw new Exception("Failed to Update Transaction Status in Tokopodia");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}