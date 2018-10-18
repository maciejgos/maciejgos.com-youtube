using Microsoft.WindowsAzure.Storage.Table;

namespace MacSamples.Models
{
    public class MovieInfo : TableEntity
    {
        public string Title { get; set; }

        public MovieInfo() {}
        public MovieInfo(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }
    }
}