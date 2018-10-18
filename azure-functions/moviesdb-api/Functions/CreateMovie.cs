using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MacSamples.Models;
using Microsoft.WindowsAzure.Storage.Table;

namespace MacSamples.Functions
{
    public static class CreateMovie
    {
        [FunctionName("CreateMovie")]
        [StorageAccount("macsamples")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "movie")] HttpRequest req,
            [Table("Movies", Connection = "AzureWebJobsStorage")]CloudTable movies,
            ILogger log)
        {
            log.LogInformation($"{nameof(CreateMovie)} function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            log.LogInformation($"Request body: {requestBody}");

            if (string.IsNullOrWhiteSpace(requestBody))
            {
                return new BadRequestObjectResult("Request body cannot be empty.");
            }

            await movies.CreateIfNotExistsAsync();

            Movie movieToInsert = JsonConvert.DeserializeObject<Movie>(requestBody);
            movieToInsert.PartitionKey = movieToInsert.Type;
            movieToInsert.RowKey = movieToInsert.Title;

            MovieInfo movieInfo = new MovieInfo(movieToInsert.Type, movieToInsert.Premiere)
            {
                Title = movieToInsert.Title
            };

            TableBatchOperation batchOperation = new TableBatchOperation();
            batchOperation.Insert(movieToInsert);
            batchOperation.Insert(movieInfo);

            var result = await movies.ExecuteBatchAsync(batchOperation);

            return (ActionResult)new OkResult();
        }
    }
}
