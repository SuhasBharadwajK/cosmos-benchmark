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
                var totalBlogs = 10;
                var commentsPerBlog = 10;
                var initialBlogSizeInKilobytes = 10;
                var blogSizeMultiplier = 10;
                var intialCommentSizeInBytes = 100;
                var commentSizeMultiplier = 10;
                var numberOfSizeMultiplications = 3;

                CosmosBenchmarkRunner cosmosBenchmarkRunner = CosmosBenchmarkRunner.Initialize(totalBlogs, commentsPerBlog, initialBlogSizeInKilobytes, blogSizeMultiplier, intialCommentSizeInBytes, commentSizeMultiplier, numberOfSizeMultiplications);
                Console.WriteLine("Intialized Cosmos Benchmark Runner");
                cosmosBenchmarkRunner.DeleteAllItems().GetAwaiter().GetResult();
                CsvGenerator csvGenerator = new CsvGenerator();
               
                Console.WriteLine("Running Benchmark for creating blogs...");
                var creationBenchmark = cosmosBenchmarkRunner.BenchmarkCreatingBlogs().GetAwaiter().GetResult();
                int blogSize = creationBenchmark.BenchmarkResults.First().BlogGenerationResult.BlogSizeInKilobytes;
                int commentSize = creationBenchmark.BenchmarkResults.First().BlogGenerationResult.CommentSizeInBytes;
                csvGenerator.GenerateBenchmarkCSV(creationBenchmark);

                Console.WriteLine("Running Benchmark for updating blogs...");
                var updationBenchmark = cosmosBenchmarkRunner.BenchmarkUpdatingBlogs().GetAwaiter().GetResult();
                csvGenerator.GenerateBenchmarkCSV(updationBenchmark);
                
                var previousBenchmarkResults = creationBenchmark.BenchmarkResults;

                Console.WriteLine("Running Benchmark for Get All Blogs Without Comments...");
                var benchmarkGetBlogsWithoutComments = cosmosBenchmarkRunner.BenchmarkGettingAllBlogsWithoutComments(previousBenchmarkResults).GetAwaiter().GetResult();
                csvGenerator.GenerateBenchmarkCSV(benchmarkGetBlogsWithoutComments);

                Console.WriteLine("Running Benchmark for Get All Blogs With Comments...");
                var benchmarkGetBlogsWithComments = cosmosBenchmarkRunner.BenchmarkGettingAllBlogsWithAllComments(previousBenchmarkResults).GetAwaiter().GetResult();
                csvGenerator.GenerateBenchmarkCSV(benchmarkGetBlogsWithComments);

                var randomEmbeddedBlogBenchmarkResult = creationBenchmark.GetRandomBenchmarkResult(Models.BlogType.Embedded);
                var randomReferentialBlogBenchmarkResult = creationBenchmark.GetRandomBenchmarkResult(Models.BlogType.Referential);
                var embeddedId = randomEmbeddedBlogBenchmarkResult.EmbeddedBlogResponse.Item.Id;
                var referenceId = randomReferentialBlogBenchmarkResult.ReferentialBlogResponse.Item.Id;


                Console.WriteLine("Running Benchmark for Getting a Blog...");
                var benchmarkGetABlog = cosmosBenchmarkRunner.BenchmarkGettingBlog(referenceId, embeddedId, previousBenchmarkResults).GetAwaiter().GetResult();
                csvGenerator.GenerateBenchmarkCSV(benchmarkGetABlog);

                Console.WriteLine("Running Benchmark for Getting a Blog with all comments...");
                var benchmarkGetBlogWithComments = cosmosBenchmarkRunner.BenchmarkGettingBlogWithAllComments(referenceId, embeddedId, previousBenchmarkResults).GetAwaiter().GetResult();
                csvGenerator.GenerateBenchmarkCSV(benchmarkGetBlogWithComments);

                Console.WriteLine("Running Benchmark for Getting a Blog with some comments...");
                var benchmarkGetBlogWithSome = cosmosBenchmarkRunner.BenchmarkGettingBlogWithSomeComments(referenceId, embeddedId, 10, previousBenchmarkResults).GetAwaiter().GetResult();
                csvGenerator.GenerateBenchmarkCSV(benchmarkGetBlogWithSome);

                Console.WriteLine("Running Benchmark for Updating Comments in a blog...");
                var benchmarkUpdatingComments = cosmosBenchmarkRunner.BenchmarkUpdatingComments(referenceId, embeddedId, 5, previousBenchmarkResults).GetAwaiter().GetResult();
                csvGenerator.GenerateBenchmarkCSV(benchmarkUpdatingComments);

                string fileName = $".\\Benchmark-{DateTime.Now.ToString("hh-mm-ss-dd-MM-yyyy")}.csv";
                csvGenerator.CreateFileIfNotExists(fileName);
                csvGenerator.WriteRecords(fileName);
                Console.WriteLine("Benchmarking has been done successfully");
            }
            catch(CosmosException e)
            {
                Console.WriteLine("Running Benchmark has been failed with cosmos exception" + e.Message);
            }
            catch(Exception e)
            {
                Console.WriteLine("Running Benchmark has been failed with some exception" + e.Message);
            }

            Console.WriteLine("Press any key to continue..");
            Console.ReadKey();
        }
    }
}
