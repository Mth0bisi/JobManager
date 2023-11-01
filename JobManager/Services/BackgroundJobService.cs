using JobManager.Data.Repository;
using JobManager.Models;
using Serilog;
using System.Text.Json;

namespace JobManager.Services
{
    public class BackgroundJobService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public BackgroundJobService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {

                    Log.Information("Initiating background job task...");
                    var jobRepository = scope.ServiceProvider.GetRequiredService<IGenericRepository<Job>>();
                    var jobs = jobRepository.GetAll();

                    if (jobs.Count == 0)
                        Log.Information($"There are no background jobs to run.");
                    else
                    {
                        Log.Information($"Task will run {jobs.Count} jobs.");
                        foreach (var job in jobs)
                        {
                            if (job.LastRunTime.AddMinutes(job.Interval) <= DateTime.Now)
                            {
                                Log.Information($"Job {job.Name} executing...");

                                await Task.Run(async () =>
                                {
                                    var httpClient = new HttpClient();
                                    var response = await httpClient.GetAsync(job.Name);

                                    if (response.IsSuccessStatusCode)
                                    {
                                        try
                                        {
                                            var jsonData = await response.Content.ReadAsStreamAsync();
                                            var weatherForecastList = await JsonSerializer.DeserializeAsync<List<WeatherForecast>>(jsonData, new JsonSerializerOptions
                                            {
                                                PropertyNameCaseInsensitive = true
                                            });

                                            if (weatherForecastList != null)
                                            {
                                                var weatheRepository = scope.ServiceProvider.GetRequiredService<IGenericRepository<WeatherForecast>>();
                                                await weatheRepository.CreateRange(weatherForecastList);
                                                await weatheRepository.Save();
                                            }

                                            job.LastRunTime = DateTime.Now;
                                            jobRepository.Update(job);
                                            await jobRepository.Save();
                                        }
                                        catch (Exception ex)
                                        {
                                            Log.Information($"There was an error while fecthing data for job {job.Name} with error {ex.Message}.");
                                            throw new Exception(ex.Message);
                                        }

                                    }
                                    else
                                    {
                                        Log.Information($"Fecthing job {job.Name} data failed.");
                                        throw new Exception("Failed to fetch data from the API.");
                                    }
                                });
                            }
                        }

                    }
                    await Task.Delay(TimeSpan.FromMinutes(20), stoppingToken);

                }

            }
        }
    }
}

