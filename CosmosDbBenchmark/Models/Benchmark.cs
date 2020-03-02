using System;
using System.Collections.Generic;
using System.Linq;

namespace CosmosDbBenchmark.Models
{
    public class Benchmark
    {
        public Benchmark(BenchmarkType benchmarkType)
        {
            this.BenchmarkResults = new List<BenchmarkResult>();
            this.BenchmarkType = benchmarkType;
        }

        public List<BenchmarkResult> BenchmarkResults { get; set; }

        public BenchmarkType BenchmarkType { get; set; }

        public void AddResult(BenchmarkResult benchmarkResult)
        {
            this.BenchmarkResults.Add(benchmarkResult);
        }

        public BenchmarkResult GetRandomBenchmarkResult(BlogType blogType)
        {
            Random rnd = new Random();

            if (blogType == BlogType.Embedded)
            {
                var embeddedResponses = this.BenchmarkResults.Where(r => r.EmbeddedBlogResponse != null).ToList();
                return embeddedResponses[rnd.Next(embeddedResponses.Count)];
            }
            else
            {
                var referentialResponses = this.BenchmarkResults.Where(r => r.ReferentialBlogResponse != null).ToList();
                return referentialResponses[rnd.Next(referentialResponses.Count)];
            }
        }
    }
}
