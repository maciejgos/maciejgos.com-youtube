using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
using MacSamples.Models;

namespace MacSamples.Functions
{
    public static class GetMovies
    {
        [FunctionName("GetMovies")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "movies/{type}")] HttpRequest req,
            [Table("Movies", Connection = "AzureWebJobsStorage")]CloudTable movies,
            string type,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if (string.IsNullOrWhiteSpace(type))
            {
                return new BadRequestObjectResult("Input parameters cannot be null.");
            }

            TableQuery<Movie> query = new TableQuery<Movie>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, type));
            var result = await movies.ExecuteQuerySegmentedAsync<Movie>(query, null);

            return (ActionResult) new OkObjectResult(result.Results);
        }
    }
}
