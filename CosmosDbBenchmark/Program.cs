using Microsoft.Azure.Cosmos;
using System;
using System.Linq;

namespace CosmosDbBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                CosmosBenchmarkRunner cosmosBenchmarkRunner = CosmosBenchmarkRunner.Initialize(2, 5, 10, 1, 100, 1, 5);
                Console.WriteLine("Intialized Cosmos Benchmark Runner");
                cosmosBenchmarkRunner.DeleteAllItems().GetAwaiter().GetResult();
                CsvGenerator csvGenerator = new CsvGenerator();
               
                Console.WriteLine("Running Benchmark for creating blogs...");
                var benchmarkCreateResults = cosmosBenchmarkRunner.BenchmarkCreatingBlogs().GetAwaiter().GetResult();
                int blogSize = benchmarkCreateResults.BenchmarkResults.First().BlogGenerationResult.BlogSizeInKilobytes;
                int commentSize = benchmarkCreateResults.BenchmarkResults.First().BlogGenerationResult.CommentSizeInBytes;
                csvGenerator.GenerateBenchmarkCSV(benchmarkCreateResults, blogSize, commentSize);
               
                Console.WriteLine("Running Benchmark for updating blogs...");
                var benchmarkUpdateResults = cosmosBenchmarkRunner.BenchmarkUpdatingBlogs().GetAwaiter().GetResult();
                csvGenerator.GenerateBenchmarkCSV(benchmarkUpdateResults, blogSize, commentSize);
                
                Console.WriteLine("Running Benchmark for Get All Blogs Without Comments...");
                var benchmarkGetBlogsWithoutComments = cosmosBenchmarkRunner.BenchmarkGettingAllBlogsWithoutComments().GetAwaiter().GetResult();
                csvGenerator.GenerateBenchmarkCSV(benchmarkGetBlogsWithoutComments, blogSize, commentSize);

                Console.WriteLine("Running Benchmark for Get All Blogs With Comments...");
                var benchmarkGetBlogsWithComments = cosmosBenchmarkRunner.BenchmarkGettingAllBlogsWithAllComments().GetAwaiter().GetResult();
                csvGenerator.GenerateBenchmarkCSV(benchmarkGetBlogsWithComments, blogSize, commentSize);
                var embeddedId = benchmarkGetBlogsWithComments.BenchmarkResults.First().EmbeddedBlogResponse.Item.Id;
                var referenceId = benchmarkGetBlogsWithComments.BenchmarkResults.Last().ReferentialBlogResponse.Item.Id;
                
                Console.WriteLine("Running Benchmark for Getting a Blog...");
                var benchmarkGetABlog = cosmosBenchmarkRunner.BenchmarkGettingBlog(referenceId, embeddedId).GetAwaiter().GetResult();
                csvGenerator.GenerateBenchmarkCSV(benchmarkGetABlog, blogSize, commentSize);

                Console.WriteLine("Running Benchmark for Getting a Blog with all comments...");
                var benchmarkGetBlogWithComments = cosmosBenchmarkRunner.BenchmarkGettingBlogWithAllComments(referenceId, embeddedId).GetAwaiter().GetResult();
                csvGenerator.GenerateBenchmarkCSV(benchmarkGetBlogWithComments, blogSize, commentSize);

                Console.WriteLine("Running Benchmark for Getting a Blog with some comments...");
                var benchmarkGetBlogWithSome = cosmosBenchmarkRunner.BenchmarkGettingBlogWithSomeComments(referenceId, embeddedId, 10).GetAwaiter().GetResult();
                csvGenerator.GenerateBenchmarkCSV(benchmarkGetBlogWithSome, blogSize, commentSize);

                Console.WriteLine("Running Benchmark for Updating Comments in a blog...");
                var benchmarkUpdatingComments = cosmosBenchmarkRunner.BenchmarkUpdatingComments(referenceId, embeddedId, 5).GetAwaiter().GetResult();
                csvGenerator.GenerateBenchmarkCSV(benchmarkUpdatingComments, blogSize, commentSize);

                string fileName = @"E:\Benchmark.csv";
                csvGenerator.CreateFileIfNotExists(fileName);
                csvGenerator.WriteRecords(fileName);
                Console.WriteLine("Benchmarking has been done successfully");
            }
            catch(CosmosException e)
            {
                Console.WriteLine("Running Benchmark has been failed with cosmos exception");
            }
            catch(Exception e)
            {
                Console.WriteLine("Running Benchmark has been failed with some exception");
            }
            Console.WriteLine("Press any key to continue..");
            Console.ReadKey();
        }
    }
}
