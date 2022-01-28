using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
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
        private GraphQLHttpClientOptions graphqlOptions;

        public TokopodiaDataClient(IConfiguration configuration)
        {
            _configuration = configuration;

            graphqlOptions = new GraphQLHttpClientOptions
            {
                EndPoint = new Uri(_configuration["TokopodiaURI"], UriKind.Absolute)
            };

            _client = new GraphQLHttpClient(
                graphqlOptions,
                new NewtonsoftJsonSerializer()
            );
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