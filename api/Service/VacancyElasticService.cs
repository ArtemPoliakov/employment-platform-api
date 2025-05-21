using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Configuration;
using api.CustomException.VacancyExceptions;
using api.Dtos.VacancyDtos.ElasticDtos;
using api.Helpers;
using api.Interfaces;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.Extensions.Options;

namespace api.Service
{
    /// <summary>
    /// Service for operating with vacancies in ElasticSearch
    /// </summary>
    public class VacancyElasticService : IVacancyElasticService
    {
        private const string INDEX_NAME = "vacancies";
        private readonly ElasticsearchClient _elasticClient;
        private readonly ElasticSettings _elasticSettings;

        public VacancyElasticService(IOptions<ElasticSettings> optionsMonitor)
        {
            _elasticSettings = optionsMonitor.Value;

            var settings = new ElasticsearchClientSettings(new Uri(_elasticSettings.Url))
                           .DefaultIndex(INDEX_NAME);

            _elasticClient = new ElasticsearchClient(settings);
        }

        /// <summary>
        /// Adds a new vacancy document to the ElasticSearch index or updates an existing one.
        /// </summary>
        /// <param name="vacancy">The vacancy document to add or update.</param>
        /// <returns>True if the operation was successful; otherwise, false.</returns>
        public async Task<bool> AddOrUpdateVacancyAsync(VacancyElasticDto vacancy)
        {
            var response = await _elasticClient.IndexAsync(vacancy);

            return response.IsValidResponse;
        }

        /// <summary>
        /// Adds new or updates existing vacancy documents in the ElasticSearch index in bulk.
        /// </summary>
        /// <param name="vacancies">The list of vacancy documents to add or update.</param>
        /// <returns>True if the operation was successful; otherwise, false.</returns>
        public async Task<bool> AddOrUpdateVacancyBulkAsync(IEnumerable<VacancyElasticDto> vacancies)
        {
            var response = await _elasticClient.BulkAsync(
                b => b.UpdateMany(vacancies, (ud, u) => ud.Doc(u).DocAsUpsert(true))
                );
            if (!response.IsValidResponse)
            {
                response.TryGetOriginalException(out var exception);
                throw new VacancyElasticException(exception?.Message ?? "Unknown Elsatic error"); //remove for prod
            }
            return response.IsValidResponse;
        }

        /// <summary>
        /// Creates a new index with the name <see cref="INDEX_NAME"/> if it does not exist.
        /// </summary>
        public async Task CreateIndexIfNotExistsAsync()
        {
            var existsResponse = await _elasticClient.Indices.ExistsAsync(INDEX_NAME);
            if (!existsResponse.Exists)
                await _elasticClient.Indices.CreateAsync(INDEX_NAME);
        }

        /// <summary>
        /// Deletes the document with the given <paramref name="key"/> from the ElasticSearch index.
        /// </summary>
        /// <param name="key">The id of the document to delete.</param>
        /// <returns>True if the delete operation was successful; otherwise, false.</returns>
        public async Task<bool> DeleteVacancyByIdAsync(string key)
        {
            var response = await _elasticClient.DeleteAsync<VacancyElasticDto>(key);

            return response.IsValidResponse;
        }

        /// <summary>
        /// Retrieves all the documents from the ElasticSearch index with the name <see cref="INDEX_NAME"/>.
        /// </summary>
        /// <returns>The list of documents if the operation was successful; otherwise, null.</returns>
        public async Task<List<VacancyElasticDto>?> GetAllVacanciesAsync()
        {
            var response = await _elasticClient.SearchAsync<VacancyElasticDto>(s => s.Index(INDEX_NAME));

            return response.IsValidResponse ? response.Documents.ToList() : default;
        }

        /// <summary>
        /// Retrieves a document from the ElasticSearch index with the given <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The id of the document to retrieve.</param>
        /// <returns>The document if it exists; otherwise, null.</returns>
        public async Task<VacancyElasticDto?> GetVacancyAsync(string key)
        {
            var response = await _elasticClient.GetAsync<VacancyElasticDto>(key);

            return response.Source;
        }

        /// <summary>
        /// Deletes all documents from the ElasticSearch index with the name <see cref="INDEX_NAME"/>.
        /// </summary>
        /// <returns>The number of documents deleted if the operation was successful; otherwise, null.</returns>
        /// <exception cref="VacancyElasticException">If the delete operation fails.</exception>
        public async Task<long?> RemoveAllAsync()
        {
            var response = await _elasticClient.DeleteByQueryAsync<VacancyElasticDto>(
            d => d
            .Indices(INDEX_NAME)
            .Query(q => q.MatchAll(new MatchAllQuery())));
            if (!response.IsValidResponse)
            {
                throw new VacancyElasticException("Failed to delete companies from elastic");
            }
            return response.Deleted;
        }

        /// <summary>
        /// Searches for vacancies by given query and returns a list of ElasticSearch documents.
        /// </summary>
        /// <param name="query">The query to search by.</param>
        /// <returns>The list of documents if the operation was successful.</returns>
        /// <exception cref="VacancyElasticException">If the search operation fails.</exception>
        public async Task<List<VacancyElasticDto>?> SearchVacanciesByQueryAsync(VacancyQueryDto query)
        {
            var mustQueries = new List<Query>();

            if (!string.IsNullOrWhiteSpace(query.Position))
            {
                mustQueries.Add(new MatchQuery("position")
                {
                    Query = query.Position,
                    Fuzziness = new Fuzziness("auto")
                });
            }

            if (!string.IsNullOrWhiteSpace(query.GeneralDescription))
            {
                var shouldQueries = new List<Query>
                {
                    new MatchQuery("title")
                    {
                        Query = query.GeneralDescription,
                        Fuzziness = new Fuzziness("auto"),
                        Boost = 2.0f
                    },
                    new MatchQuery("description")
                    {
                        Query = query.GeneralDescription,
                        Fuzziness = new Fuzziness("auto")
                    }
                };

                mustQueries.Add(new BoolQuery
                {
                    Should = shouldQueries,
                    MinimumShouldMatch = 1
                });
            }

            mustQueries.Add(new NumberRangeQuery("minSalary")
            {
                Gte = query.MinSalary,
                Lte = query.MaxSalary
            });

            mustQueries.Add(new NumberRangeQuery("maxSalary")
            {
                Gte = query.MinSalary,
                Lte = query.MaxSalary
            });


            if (!(string.IsNullOrWhiteSpace(query.WorkMode) || query.WorkMode.ToUpper() == "NONE"))
            {
                mustQueries.Add(new TermQuery("workMode.keyword")
                {
                    Value = query.WorkMode.ToUpper()
                });
            }

            var searchRequest = new SearchRequest<VacancyElasticDto>
            {
                From = (query.Page - 1) * query.PageSize,
                Size = query.PageSize,
                Query = new BoolQuery
                {
                    Must = mustQueries
                }
            };

            var response = await _elasticClient.SearchAsync<VacancyElasticDto>(searchRequest);
            if (!response.IsValidResponse)
            {
                response.TryGetOriginalException(out var exception);
                throw new VacancyElasticException(exception?.Message ?? "Unknown Elsatic error"); //remove for prod
            }
            return response.Documents.ToList();
        }
    }
}