using System;
using System.Collections.Generic;
using System.Linq;
using CosmosDbBenchmark.Generations;
using CosmosDbBenchmark.Models;

namespace CosmosDbBenchmark
{
    public class CosmosBenchmarkRunner
    {
        private EmbeddedOperations _embeddedOperations;

        private ReferentialOperations _referentialOperations;

        private BlogGenerator _blogGenerator;

        public CosmosBenchmarkRunner()
        {
            this._embeddedOperations = new EmbeddedOperations();
            this._referentialOperations = new ReferentialOperations();
        }

        public static CosmosBenchmarkRunner Initialize(
            int blogCount,
            int commentsPerBlog,
            int initialBlogSizeInKilobytes,
            int blogSizeMultiplier,
            int intialCommentSizeInBytes,
            int commentSizeMultiplier,
            int numberOfMultiplications
            )
        {
            var benchmarkRunner = new CosmosBenchmarkRunner();
            benchmarkRunner._blogGenerator = new BlogGenerator(blogCount, commentsPerBlog, initialBlogSizeInKilobytes, blogSizeMultiplier, intialCommentSizeInBytes, commentSizeMultiplier, numberOfMultiplications);
            return benchmarkRunner;
        }

        // Create n blogs with m comments each.
        public Benchmark BenchmarkCreatingBlogs()
        {
            var benchmark = new Benchmark(BenchmarkType.CreateAllBlogs);
            var blogGenerationResults = this._blogGenerator.GenerateBlogsWithComments();

            var blogCount = 1;

            blogGenerationResults.ForEach(async result =>
            {
                var benchmarkResult = new BenchmarkResult { BlogGenerationResult = result };

                var newBlog = new Blog
                {
                    Id = Guid.NewGuid().ToString(),
                    BlogType = result.BlogType,
                    Content = result.BlogText,
                    Title = Constants.BlogTitlePrefix + " " + blogCount,
                    Name = Constants.BlogNamePrefix + " " + blogCount,
                    CreatedOn = DateTime.UtcNow,
                    BlogComments = result.Comments.Select(generatedComment => new Comment {
                        AuthorName = TextHelper.GetRandomName(),
                        CommentedOn = DateTime.UtcNow,
                        CommentText = generatedComment
                    }).ToList()
                };

                if (result.BlogType == BlogType.Embedded)
                {
                    benchmarkResult.EmbeddedBlogResponse = await this._embeddedOperations.CreateBlog((EmbeddedBlog)newBlog);
                }
                else
                {
                    benchmarkResult.ReferentialBlogResponse = await this._referentialOperations.CreateBlog(newBlog);
                }

                benchmark.BenchmarkResults.Add(benchmarkResult);

                blogCount++;
            });

            return benchmark;
        }

        // Update n blogs
        public Benchmark BenchmarkUpdatingBlogs()
        {
            var benchmark = new Benchmark(BenchmarkType.UpdateAllBlogs);
            // TODO.
            var blogGenerationResults = this._blogGenerator.GenerateBlogsWithComments();
            // Get all blogs and record benchmarks

            // Update all blogs and record benchmarks

            return benchmark;
        }

        // Update n comments
        public Benchmark BenchmarkUpdatingComments(string blogId, int commentsCount)
        {
            // TODO.
            var benchmark = new Benchmark(BenchmarkType.UpdateAllCommentsInABlog);
            return benchmark;
        }

        // Get all blogs
        public Benchmark BenchmarkGettingAllBlogs()
        {
            // TODO.
            var benchmark = new Benchmark(BenchmarkType.GetAllBlogs);
            return benchmark;
        }

        // Get all blogs with all comments
        public Benchmark BenchmarkGettingAllBlogsWithAllComments()
        {
            // TODO.
            // Both referential and embedded including comments.
            var benchmark = new Benchmark(BenchmarkType.GetAllBlogsWithAllComments);
            return benchmark;
        }

        // Get one blog
        public Benchmark BenchmarkGettingBlog(string blogId)
        {
            var benchmark = new Benchmark(BenchmarkType.GetOneBlog);
            // TODO.
            return benchmark;
        }

        // Get one blogs with all comments
        public Benchmark BenchmarkGettingBlogWithAllComments(string blogId)
        {
            var benchmark = new Benchmark(BenchmarkType.GetOneBlogWithAllComments);
            // TODO.
            return benchmark;
        }

        // Get one blogs with n comments
        public Benchmark BenchmarkGettingBlogWithSomeComments(string blogId, int numberOfComments)
        {
            var benchmark = new Benchmark(BenchmarkType.GetOneBlogWithSomeComments);
            // TODO.
            return  benchmark;
        }
    }
}