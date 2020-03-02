using CosmosDbBenchmark.Models;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace CosmosDbBenchmark
{
    public class CsvGenerator
    {
        public CsvGenerator()
        {
            BenchmarkCsvs = new List<BenchmarkCsv>();
        }
        public List<BenchmarkCsv> BenchmarkCsvs { get; set; }

        public void WriteRecords(string fileName)
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            using (CsvWriter cw = new CsvWriter(sw, CultureInfo.InvariantCulture))
            {
                cw.WriteRecords(BenchmarkCsvs);  
            }
        }

        public void CreateFileIfNotExists(string fileName)
        {
            if (!File.Exists(fileName))
            {
                var file = File.Create(fileName);
                file.Close();
            }
        }

        public void GenerateBenchmarkCSV(Benchmark benchmark, int blogSize, int commentSize)
        {
            List<BenchmarkCsv> benchmarkCsvs = new List<BenchmarkCsv>();

            foreach (var result in benchmark.BenchmarkResults)
            {
                if (result.EmbeddedBlogResponse != null)
                {
                    benchmarkCsvs.Add(new BenchmarkCsv
                    {

                        BlogType = BlogType.Embedded.ToString(),
                        Entitytype = Constants.BlogTypeKey,
                        TypeOfOperation = result.EmbeddedBlogResponse.ComsosDbOperation.ToString(),
                        Size = blogSize + " Kb",
                        ConsumedRU = result.EmbeddedBlogResponse.RequestCharge
                    });
                }
                else
                {
                    benchmarkCsvs.Add(new BenchmarkCsv
                    {

                        BlogType = BlogType.Referential.ToString(),
                        Entitytype = Constants.BlogTypeKey,
                        TypeOfOperation = result.ReferentialBlogResponse.ComsosDbOperation.ToString(),
                        Size = blogSize + " Kb",
                        ConsumedRU = result.ReferentialBlogResponse.RequestCharge
                    });
                    if (result.ChildBenchmarkResults != null && result.ChildBenchmarkResults.Count > 0)
                    {
                        foreach (var childResult in result.ChildBenchmarkResults)
                        {
                            benchmarkCsvs.Add(new BenchmarkCsv
                            {

                                BlogType = BlogType.Referential.ToString(),
                                Entitytype = Constants.CommentTypeKey,
                                TypeOfOperation = childResult.ReferentialCommentResponse.ComsosDbOperation.ToString(),
                                Size = commentSize + " bytes",
                                ConsumedRU = childResult.ReferentialCommentResponse.RequestCharge
                            });
                        }
                    }
                }

            }
            BenchmarkCsvs.AddRange(benchmarkCsvs);
        }
    }
}
