using System;

namespace CosmosDbBenchmark.Models
{
    public class Comment
    {
        public string commentText { get; set; }

        public string authorName { get; set; }

        public DateTime commentedOn { get; set; }
    }
}