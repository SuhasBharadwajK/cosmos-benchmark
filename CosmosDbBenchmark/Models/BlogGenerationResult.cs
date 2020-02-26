using System.Collections.Generic;

namespace CosmosDbBenchmark.Models
{
    public class BlogGenerationResult
    {
        public int BlogSizeInKilobytes { get; set; }

        public int CommentSizeInBytes { get; set; }

        public string BlogText { get; set; }

        public List<string> Comments { get; set; }
    }
}
