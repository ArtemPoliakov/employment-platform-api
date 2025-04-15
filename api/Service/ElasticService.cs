using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Configuration;
using api.Dtos.JobseekerDto.ElasticDtos;
using api.Interfaces;
using api.Models;
using Elastic.Clients.Elasticsearch;
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
                d => d.Indices(INDEX_NAME));

            return response.IsValidResponse ? response.Deleted : null;
        }
    }
}