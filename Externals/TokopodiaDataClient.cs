using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DiAnterExpress.Models;
using GraphQL;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;

namespace DiAnterExpress.Externals
{
    public class TokopodiaDataClient : ITokopodiaDataClient
    {
        private GraphQLHttpClient _client;

        public TokopodiaDataClient(GraphQLHttpClient client)
        {
            _client = client;
        }

        public async Task ShipmentDelivered(int id, string token)
        {
            var query = new GraphQLRequest
            {
                Query =
                @"
                    mutation {
                        updateTransaction(
                            {
                                transactionId: $transactionId
                            }
                        )
                    }
                ",
                Variables = new { transactionId = id },
            };

            _client.HttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            //var response = await _client.SendMutationAsync<>(query);
            // Nunggu konfirmasi bentuk return datanya kayak gimana
        }
    }
}