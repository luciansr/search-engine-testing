﻿using System;
using System.Collections.Generic;
using System.Linq;
using Elasticsearch.Net;
using Models;
using Nest;

namespace Elastic.Repository
{
    public class ElasticRepository
    {
        private IElasticClient _elasticClient;

        public ElasticRepository(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public void AddQueryIfNotExistent(string query)
        {
            var resultQuery = SearchExactQuery(query);

            if (resultQuery == null || resultQuery.FirstOrDefault()?.query != query)
            {
                AddQuery(query);
            }
        }

        private void AddQuery(string query)
        {
            // var result = _elasticClient.IndexDocument(new Query
            // {
            //     query = query
            // });
            
            _elasticClient.LowLevel.Index<BytesResponse>("search", "query", PostData.Serializable(new Query
            {
                query = query
            }));
        }

        public IEnumerable<Offer> SearchOffer(String query)
        {
            AddQueryIfNotExistent(query);

            var result = _elasticClient.LowLevel.Search<SearchResponse<Offer>>("index_offer", "offer", PostData.Serializable(new
            {
                from = 0,
                //taking 'all' results to the neural network decide which is best to show
                size = 1000,
                query = new
                {
                    match = new
                    {
                        title = new
                        {
                            query = query,
                            fuzziness = 2,
                            prefix_length = 1
                        }
                    }
                }
            }));

            if (result.IsValid)
            {
                return result.Documents;
            }

            return null;
        }

        public IEnumerable<Query> SearchExactQuery(String query)
        {
            var result = _elasticClient.LowLevel.Search<SearchResponse<Query>>("search", "query", PostData.Serializable(new
            {
                from = 0,
                size = 11,
                query = new
                {
                    match = new
                    {
                        query = new
                        {
                            query = query,
                            fuzziness = 2,
                            prefix_length = 1
                        }
                    }
                }
            }));

            if (result.IsValid)
            {
                return result.Documents;
            }

            return null;
        }
    }
}
