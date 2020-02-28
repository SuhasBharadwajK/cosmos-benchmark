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
                Console.WriteLine("Running Benchmark for creating blogs...");
                var benchmarkCreateResults = cosmosBenchmarkRunner.BenchmarkCreatingBlogs().GetAwaiter().GetResult();
                Console.WriteLine("Running Benchmark for updating blogs...");
                var benchmarkUpdateResults = cosmosBenchmarkRunner.BenchmarkUpdatingBlogs().GetAwaiter().GetResult();
                Console.WriteLine("Running Benchmark for Get All Blogs Without Comments...");
                var benchmarkGetBlogsWithoutComments = cosmosBenchmarkRunner.BenchmarkGettingAllBlogsWithoutComments().GetAwaiter().GetResult();
                Console.WriteLine("Running Benchmark for Get All Blogs With Comments...");
                var benchmarkGetBlogsWithComments = cosmosBenchmarkRunner.BenchmarkGettingAllBlogsWithAllComments().GetAwaiter().GetResult();
                var embeddedId = benchmarkGetBlogsWithComments.BenchmarkResults.First().EmbeddedBlogResponse.Item.Id;
                var referenceId = benchmarkGetBlogsWithComments.BenchmarkResults.Last().ReferentialBlogResponse.Item.Id;
                Console.WriteLine("Running Benchmark for Getting a Blog...");
                var benchmarkGetABlog = cosmosBenchmarkRunner.BenchmarkGettingBlog(referenceId, embeddedId);
                Console.WriteLine("Running Benchmark for Getting a Blog with all comments...");
                var benchmarkGetBlogWithComments = cosmosBenchmarkRunner.BenchmarkGettingBlogWithAllComments(referenceId, embeddedId).GetAwaiter().GetResult();
                Console.WriteLine("Running Benchmark for Getting a Blog with some comments...");
                var benchmarkGetBlogWithSome = cosmosBenchmarkRunner.BenchmarkGettingBlogWithSomeComments(referenceId, embeddedId, 10).GetAwaiter().GetResult();
                Console.WriteLine("Running Benchmark for Updating Comments in a blog...");
                var benchmarkUpdatingComments = cosmosBenchmarkRunner.BenchmarkUpdatingComments(referenceId, embeddedId, 5).GetAwaiter().GetResult();
                Console.WriteLine("Benchmarking has been done successfully");
            }
            catch(CosmosException e)
            {
                Console.WriteLine("Running Benchmark has been failed with some exception");
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
