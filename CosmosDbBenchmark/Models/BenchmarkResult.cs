using System.Collections.Generic;

namespace CosmosDbBenchmark.Models
{
    public class BenchmarkResult
    {
        public BenchmarkResult()
        {
        }

        public BlogGenerationResult BlogGenerationResult { get; set; }

        public CommentGenerationResult CommentGenerationResult { get; set; }

        public CosmosResponse<EmbeddedBlog> EmbeddedBlogResponse { get; set; }

        public CosmosResponse<ReferentialBlog> ReferentialBlogResponse { get; set; }

        public CosmosResponse<ReferentialComment> ReferentialCommentResponse { get; set; }
    }
}
