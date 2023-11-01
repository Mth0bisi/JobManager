using JobManager.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace JobManager.Data
{
    public class JobManagerDbContext : DbContext
    {
        public JobManagerDbContext(DbContextOptions<JobManagerDbContext> options) : base(options) { }

        public DbSet<Job> Jobs { get; set; }
        public DbSet<WeatherForecast> WeatherForecast { get; set; }
    }
}
