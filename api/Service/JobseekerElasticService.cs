using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Configuration;
using api.CustomException;
using api.Dtos.JobseekerDto.ElasticDtos;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.Extensions.Options;

namespace api.Service
{
    public class JobseekerElasticService : IJobseekerElasticService
    {
        private const string INDEX_NAME = "jobseekers";
        private readonly ElasticsearchClient _elasticClient;
        private readonly ElasticSettings _elasticSettings;

        public JobseekerElasticService(IOptions<ElasticSettings> optionsMonitor)
        {
            _elasticSettings = optionsMonitor.Value;

            var settings = new ElasticsearchClientSettings(new Uri(_elasticSettings.Url))
                           .DefaultIndex(INDEX_NAME);

            _elasticClient = new ElasticsearchClient(settings);
        }

        public async Task<bool> AddOrUpdateJobseekerAsync(JobseekerElasticDto jobseeker)
        {
            var response = await _elasticClient.IndexAsync(jobseeker);

            return response.IsValidResponse;
        }

        public async Task<bool> AddOrUpdateJobseekerBulkAsync(IEnumerable<JobseekerElasticDto> jobseekers)
        {
            var response = await _elasticClient.BulkAsync(
                b => b.UpdateMany(jobseekers, (ud, u) => ud.Doc(u).DocAsUpsert(true))
            );

            return response.IsValidResponse;
        }

        public async Task CreateIndexIfNotExistsAsync()
        {
            var existsResponse = await _elasticClient.Indices.ExistsAsync(INDEX_NAME);
            if (!existsResponse.Exists)
                await _elasticClient.Indices.CreateAsync(INDEX_NAME);
        }

        public async Task<bool> DeleteJobseekerAsync(string key)
        {
            var response = await _elasticClient.DeleteAsync<JobseekerElasticDto>(key);

            return response.IsValidResponse;
        }

        public async Task<List<JobseekerElasticDto>?> GetAllJobseekersAsync()
        {
            var response = await _elasticClient.SearchAsync<JobseekerElasticDto>(s => s.Index(INDEX_NAME));

            return response.IsValidResponse ? response.Documents.ToList() : default;
        }

        public async Task<JobseekerElasticDto?> GetJobseekerAsync(string key)
        {
            var response = await _elasticClient.GetAsync<JobseekerElasticDto>(key);

            return response.Source;
        }

        public async Task<long?> RemoveAllAsync()
        {
            var response = await _elasticClient.DeleteByQueryAsync<JobseekerElasticDto>(
                d => d
                    .Indices(INDEX_NAME)
                    .Query(q => q.MatchAll(new MatchAllQuery())));
            if (!response.IsValidResponse)
            {
                throw new JobseekerElasticException("Failed to delete jobseekers from elastic");
            }
            return response.Deleted;
        }

        public async Task<List<JobseekerElasticDto>?> SearchJobseekersByQueryAsync(JobseekerQueryDto query)
        {
            var mustQueries = new List<Query>();

            if (!string.IsNullOrWhiteSpace(query.Profession))
            {
                mustQueries.Add(new MatchQuery("profession")
                {
                    Query = query.Profession,
                    Fuzziness = new Fuzziness("auto")
                });
            }

            if (query.ExperienceMin > 0 || query.ExperienceMax < 100)
            {
                mustQueries.Add(new NumberRangeQuery("experience")
                {
                    Gte = query.ExperienceMin,
                    Lte = query.ExperienceMax
                });
            }

            if (!string.IsNullOrWhiteSpace(query.Education))
            {
                mustQueries.Add(new TermQuery("education.keyword")
                {
                    Value = query.Education.ToLower()
                });
            }

            if (!string.IsNullOrWhiteSpace(query.Location))
            {
                mustQueries.Add(new MatchQuery("location")
                {
                    Query = query.Location,
                    Fuzziness = new Fuzziness("auto")
                });
            }

            var searchRequest = new SearchRequest<JobseekerElasticDto>
            {
                From = (query.Page - 1) * query.PageSize,
                Size = query.PageSize,
                Query = new BoolQuery
                {
                    Must = mustQueries
                }
            };

            var response = await _elasticClient.SearchAsync<JobseekerElasticDto>(searchRequest);
            if (!response.IsValidResponse)
            {
                response.TryGetOriginalException(out var exception);
                throw new JobseekerSearchException(exception?.Message ?? "Unknown Elsatic error"); //remove for prod
            }
            return response.Documents.ToList();
        }
    }
}