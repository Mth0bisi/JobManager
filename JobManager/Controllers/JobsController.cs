using JobManager.Data.Repository;
using JobManager.Models;
using JobManager.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.ComponentModel.DataAnnotations;

namespace JobManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly IJobService _jobService;

        public JobsController(IJobService jobService) 
        {
            _jobService = jobService;
        }

        [HttpGet]
        public IActionResult GetJobs()
        {
            try
            {
                Log.Information("Fetching all the jobs from the database");

                var jobs = _jobService.GetJobs(); 
                Log.Information("Returning jobs => {@jobs}", jobs);

                return Ok(jobs);
            }
            catch (Exception ex)
            {
                Log.Information($"Something went wrong: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetJob(int id)
        {
            Log.Information($"Fetching job with Id {id}.");
            var job = await _jobService.GetJobById(id);
            if (job == null)
            {
                Log.Information($"Job with Id {id} not found.");
                return NotFound();
            }

            Log.Information($"Returning job with Id {id}.");
            return Ok(job);
        }

        [HttpPost]
        public async Task<IActionResult> CreateJob(Job job)
        {
            try
            {
                Log.Information($"Creating job with name {job.Name} and with interval of {job.Interval}.");
                if (string.IsNullOrWhiteSpace(job.Name) || job.Interval <= 0)
                {
                    Log.Information($"Name or interval of the job is missing");
                    throw new ValidationException("Name and Interval are required fields.");
                }

                await _jobService.CreateJob(job);

                Log.Information($"Job {job.Name} successfully created.");
                return CreatedAtAction(nameof(GetJob), new { id = job.Id }, job);
            }
            catch (ValidationException) 
            {
                Log.Information($"Validation exception thrown");
                throw;
            }
            catch (Exception ex)
            {
                Log.Information($"Something went wrong: {ex}");
                throw;
            }
        }
    }
}
