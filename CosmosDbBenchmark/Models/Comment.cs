using System;

namespace CosmosDbBenchmark.Models
{
    public class Comment
    {
        public string CommentText { get; set; }

        public string AuthorName { get; set; }

        public DateTime CommentedOn { get; set; }
    }
}