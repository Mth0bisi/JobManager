using JobManager.Data.Repository;
using JobManager.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Net.Http;

namespace JobManager.Services
{
    public interface IJobService 
    {
        List<Job> GetJobs();
        Task<Job> GetJobById(int id);
        Task<Job> CreateJob(Job job);

    }
    public class JobManagementService : IJobService
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly IGenericRepository<Job> _repository;

        public JobManagementService(IHttpClientFactory httpClient, IGenericRepository<Job> repository)
        {
            _httpClient = httpClient;
            _repository = repository;

        }
        public async Task<Job> CreateJob(Job job)
        {
            await _repository.Create(job);
            await _repository.Save();

            return job;
        }

        public async void ExecuteJobs()
        {
            var jobs = GetJobs();

            foreach (var job in jobs) 
            {
                if (job.LastRunTime.AddMinutes(job.Interval) <= DateTime.Now)
                {
                    Log.Information($"Job {job.Name} executing...");
                    await Task.Run(async() =>
                    {
                        var httpClient = _httpClient.CreateClient();
                        var response = await httpClient.GetAsync("");

                        if (response.IsSuccessStatusCode)
                        {
                            var jsonData = await response.Content.ReadAsStringAsync();

                            job.LastRunTime = DateTime.Now;
                            await _repository.Save();
                        }
                    });
                }
            }
        }

        public async Task<Job> GetJobById(int id)
        {
            return await _repository.GetById(id);
        }

        public List<Job> GetJobs()
        {
            return _repository.GetAll();
        }
    }
}
