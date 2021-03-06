﻿using System.Collections.Generic;

namespace CosmosDbBenchmark.Models
{
    public class BenchmarkResult
    {
        public BenchmarkResult()
        {
            ChildBenchmarkResults = new List<BenchmarkResult>();
        }

        public List<BenchmarkResult> ChildBenchmarkResults { get; set; }

        public BlogGenerationResult BlogGenerationResult { get; set; }

        public CommentGenerationResult CommentGenerationResult { get; set; }

        public CosmosResponse<EmbeddedBlog> EmbeddedBlogResponse { get; set; }

        public CosmosResponse<ReferentialBlog> ReferentialBlogResponse { get; set; }

        public CosmosResponse<ReferentialComment> ReferentialCommentResponse { get; set; }
    }
}
