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
        public List<BenchmarkResult> BenchmarkCreatingBlogs()
        {
            var benchmarkResults = new List<BenchmarkResult>();
            var blogGenerationResults = this._blogGenerator.GenerateBlogsWithComments();

            var blogCount = 1;

            blogGenerationResults.ForEach(async result =>
            {
                var benchmarkResult = new BenchmarkResult { BlogGenerationResult = result };

                var newBlog = new Blog
                {
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

                benchmarkResults.Add(benchmarkResult);

                blogCount++;
            });

            return benchmarkResults;
        }

        // Update n blogs
        public List<BenchmarkResult> BenchmarkUpdatingBlogs()
        {
            var benchmarkResults = new List<BenchmarkResult> { };

            // TODO.

            return benchmarkResults;
        }

        // Update n comments
        public List<BenchmarkResult> BenchmarkUpdatingComments(string blogId, int commentsCount)
        {
            var benchmarkResults = new List<BenchmarkResult> { };

            // TODO.

            return benchmarkResults;
        }

        // Get all blogs
        public List<BenchmarkResult> BenchmarkGettingAllBlogs()
        {
            var benchmarkResults = new List<BenchmarkResult> { };

            // TODO.

            return  benchmarkResults;
        }

        // Get all blogs with all comments
        public List<BenchmarkResult> BenchmarkGettingAllBlogsWithAllComments()
        {
            // TODO.
            var benchmarkResults = new List<BenchmarkResult> { };
            return  benchmarkResults;
        }

        // Get one blog
        public List<BenchmarkResult> BenchmarkGettingBlog(string blogId)
        {
            var benchmarkResults = new List<BenchmarkResult> { };
            // TODO.
            return  benchmarkResults;
        }

        // Get one blogs with all comments
        public List<BenchmarkResult> BenchmarkGettingBlogWithAllComments(string blogId)
        {
            var benchmarkResults = new List<BenchmarkResult> { };
            // TODO.
            return  benchmarkResults;
        }

        // Get one blogs with n comments
        public List<BenchmarkResult> BenchmarkGettingBlogWithSomeComments(string blogId, int numberOfComments)
        {
            var benchmarkResults = new List<BenchmarkResult> { };
            // TODO.
            return  benchmarkResults;
        }
    }
}