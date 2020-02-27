using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                    Title = Constants.BlogTitlePrefix + " " + blogCount % 2,
                    Name = Constants.BlogNamePrefix + " " + blogCount % 2,
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
        public async Task<Benchmark> BenchmarkUpdatingBlogs()
        {
            var benchmark = new Benchmark(BenchmarkType.UpdateAllBlogs);
            // TODO.
            var blogGenerationResults = this._blogGenerator.GenerateBlogsWithComments();
            // Get all blogs and record benchmarks
            var embeddedBlogs = await this._embeddedOperations.GetAllBlogs();
            int embeddedIndex = 0;
            foreach(var embeddedBlog in embeddedBlogs)
            {
                embeddedBlog.Item.Content = blogGenerationResults[embeddedIndex].BlogText;
                var benchmarkResult = new BenchmarkResult { BlogGenerationResult = blogGenerationResults[embeddedIndex] };
                benchmarkResult.EmbeddedBlogResponse = await this._embeddedOperations.UpdateBlog(embeddedBlog.Item);
                benchmark.BenchmarkResults.Add(benchmarkResult);
                embeddedIndex++;
            }
            int referentialIndex = 0;
            var referentialBlogs = await this._referentialOperations.GetAllBlogs();
            foreach (var blog in referentialBlogs)
            {
                blog.Item.Content = blogGenerationResults[referentialIndex].BlogText;
                var benchmarkResult = new BenchmarkResult { BlogGenerationResult = blogGenerationResults[referentialIndex] };
                benchmarkResult.ReferentialBlogResponse = await this._referentialOperations.UpdateBlog(blog.Item);
                benchmark.BenchmarkResults.Add(benchmarkResult);
                referentialIndex++;
            }

            return benchmark;
        }

        // Update n comments
        public async Benchmark BenchmarkUpdatingComments(string blogId, int commentsCount)
        {
            // TODO.
            var benchmark = new Benchmark(BenchmarkType.UpdateAllCommentsInABlog);
            var embeddedBlog = await this._embeddedOperations.GetBlog(blogId);
            var comments = _blogGenerator.GenrateBlogComments(commentsCount,embeddedBlog.Item.Comments.Count);

            return benchmark;
        }

        // Get all blogs
        public async Task<Benchmark> BenchmarkGettingAllBlogs()
        {
            // TODO.
            var benchmark = new Benchmark(BenchmarkType.GetAllBlogs);
            var embeddedBlogs = await this._embeddedOperations.GetAllBlogs();
            foreach (var blog in embeddedBlogs)
            {
                var benchmarkResult = new BenchmarkResult { EmbeddedBlogResponse = blog };
                benchmark.BenchmarkResults.Add(benchmarkResult);
            }
            var referentialBlogs = await this._referentialOperations.GetAllBlogs();
            foreach (var blog in referentialBlogs)
            {
                var benchmarkResult = new BenchmarkResult { ReferentialBlogResponse = blog };
                benchmark.BenchmarkResults.Add(benchmarkResult);
            }
            return benchmark;
        }

        // Get all blogs with all comments
        public async Task<Benchmark> BenchmarkGettingAllBlogsWithAllComments()
        {
            // TODO.
            // Both referential and embedded including comments.
            var benchmark = new Benchmark(BenchmarkType.GetAllBlogsWithAllComments);
            var embeddedBlogs = await this._embeddedOperations.GetAllBlogsWithAllComments();
            foreach (var blog in embeddedBlogs)
            {
                var benchmarkResult = new BenchmarkResult { EmbeddedBlogResponse = blog };
                benchmark.BenchmarkResults.Add(benchmarkResult);
            }
            var referentialBlogs = await this._referentialOperations.GetAllBlogsWithAllComments();
            foreach (var blog in referentialBlogs)
            {
                var benchmarkResult = new BenchmarkResult { ReferentialBlogResponse = blog };
                benchmark.BenchmarkResults.Add(benchmarkResult);
            }
            return benchmark;
        }

        // Get one blog
        public async Task<Benchmark> BenchmarkGettingBlog(string blogId)
        {
            var benchmark = new Benchmark(BenchmarkType.GetOneBlog);
            var embeddedBlog = await this._embeddedOperations.GetBlog(blogId);
            benchmark.BenchmarkResults.Add(new BenchmarkResult { EmbeddedBlogResponse = embeddedBlog });
            var referentialBlog = await this._referentialOperations.GetBlog(blogId);
            benchmark.BenchmarkResults.Add(new BenchmarkResult { ReferentialBlogResponse = referentialBlog });
            return benchmark;
        }

        // Get one blogs with all comments
        public async Task<Benchmark> BenchmarkGettingBlogWithAllComments(string blogId)
        {
            var benchmark = new Benchmark(BenchmarkType.GetOneBlogWithAllComments);
            var embeddedBlog = await this._embeddedOperations.GetOneBlogWithAllComments(blogId);
            benchmark.BenchmarkResults.Add(new BenchmarkResult { EmbeddedBlogResponse = embeddedBlog });
            var referentialBlog = await this._referentialOperations.GetOneBlogWithAllComments(blogId);
            benchmark.BenchmarkResults.Add(new BenchmarkResult { ReferentialBlogResponse = referentialBlog });
            return benchmark;
        }

        // Get one blogs with n comments
        public async Task<Benchmark> BenchmarkGettingBlogWithSomeComments(string blogId, int numberOfComments)
        {
            var benchmark = new Benchmark(BenchmarkType.GetOneBlogWithSomeComments);
            var embeddedBlog = await this._embeddedOperations.GetOneBlogWithSomeComments(blogId);
            benchmark.BenchmarkResults.Add(new BenchmarkResult { EmbeddedBlogResponse = embeddedBlog });
            var referentialBlog = await this._referentialOperations.GetOneBlogWithSomeComments(blogId,numberOfComments);
            benchmark.BenchmarkResults.Add(new BenchmarkResult { ReferentialBlogResponse = referentialBlog });
            return  benchmark;
        }
    }
}