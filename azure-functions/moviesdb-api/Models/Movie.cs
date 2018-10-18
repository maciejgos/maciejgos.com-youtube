using Microsoft.WindowsAzure.Storage.Table;

namespace MacSamples.Models
{
    public class Movie : MovieInfo
    {
        public string Description { get; set; }
        public string Premiere { get; set; }
        public string Type { get; set; }

        public Movie() : base() {}
        public Movie(string partitionKey, string rowKey) 
            : base(partitionKey, rowKey) {}
    }
}