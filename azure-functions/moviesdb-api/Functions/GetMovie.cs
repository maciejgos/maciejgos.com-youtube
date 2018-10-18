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

namespace MacSamples.Functions
{
    public static class GetMovie
    {
        [FunctionName("GetMovie")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "movie/{type}/{title}")] HttpRequest req,
            [Table("Movies", Connection = "AzureWebJobsStorage")]CloudTable movies,
            string type,
            string title,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if (string.IsNullOrWhiteSpace(type) || string.IsNullOrWhiteSpace(title))
            {
                return new BadRequestObjectResult("Input parameters cannot be null.");
            }

            TableOperation selectOperation = TableOperation.Retrieve(partitionKey: type, rowkey: title);
            var tableResult = await movies.ExecuteAsync(selectOperation);

            if (tableResult.Result == null)
            {
                return new OkObjectResult("No data.");
            }

            return new OkObjectResult(tableResult.Result);
        }
    }
}
