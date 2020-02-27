using System.Collections.Generic;

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
    }
}
