using API.DTOs;
using API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class JobsController : BaseApiController
{
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<List<JobDto>>> GetJobs()
    {
        var jobsList = await System.IO.File.ReadAllLinesAsync("Data/jobs.csv");
        var jobs     = new List<JobDto>();
        
        foreach (var jobData in jobsList)
        {
            var splitJobData = jobData.Split(", ");
            var job          = new JobDto()
            {
                JobTitle = splitJobData[0],
                JobCompany = splitJobData[1],
                JobLink = splitJobData[^1]
            };
            
            if(job.JobTitle.ToLower().Contains("java")) continue;
            if(job.JobTitle.ToLower().Contains("data analyst")) continue;
            if(job.JobTitle.ToLower().Contains("artificial intelligence")) continue;
            if(job.JobTitle.ToLower().Contains("data engineer")) continue;
            if(job.JobTitle.ToLower().Contains("crypto")) continue;
            if(job.JobTitle.ToLower().Contains("mobile application")) continue;
            if(job.JobTitle.ToLower().Contains("ios")) continue;
            if(job.JobTitle.ToLower().Contains("android")) continue;
            if(job.JobTitle.ToLower().Contains("php")) continue;
            jobs.Add(job);
        }

        jobs.Sort((a,b) => a.JobTitle.CompareTo(b.JobTitle));
        return jobs;
    }

}