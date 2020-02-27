using CosmosDbBenchmark.Generations;
using CosmosDbBenchmark.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<Benchmark> BenchmarkCreatingBlogs()
        {
            var benchmark = new Benchmark(BenchmarkType.CreateAllBlogs);
            var blogGenerationResults = this._blogGenerator.GenerateBlogsWithComments();

            var blogCount = 1;

            foreach (var result in blogGenerationResults)
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
                    BlogComments = result.Comments.Select(generatedComment => new Comment
                    {
                        AuthorName = TextHelper.GetRandomName(),
                        CommentedOn = DateTime.UtcNow,
                        CommentText = generatedComment
                    }).ToList()
                };

                if (result.BlogType == BlogType.Embedded)
                {
                    benchmarkResult.EmbeddedBlogResponse = await this._embeddedOperations.CreateBlog(newBlog.CastTo<EmbeddedBlog>());
                }
                else
                {
                    var createBlogResult = await this._referentialOperations.CreateBlog(newBlog.CastTo<ReferentialBlog>());
                    benchmarkResult.ReferentialBlogResponse = createBlogResult.Item1;
                    benchmarkResult.ChildBenchmarkResults = createBlogResult.Item2.Select(c => new BenchmarkResult { ReferentialCommentResponse = c }).ToList();
                }

                benchmark.BenchmarkResults.Add(benchmarkResult);

                blogCount++;
            }

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
            foreach (var embeddedBlog in embeddedBlogs)
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
        public async Task<Benchmark> BenchmarkUpdatingComments(string referenceBlogId, string embeddedBlogId, int commentsCount)
        {
            // TODO.
            var benchmark = new Benchmark(BenchmarkType.UpdateAllCommentsInABlog);
            var embeddedBlog = await this._embeddedOperations.GetBlog(embeddedBlogId);
            benchmark.BenchmarkResults.Add(new BenchmarkResult { EmbeddedBlogResponse = embeddedBlog });
            var comments = _blogGenerator.GenrateBlogComments(commentsCount, embeddedBlog.Item.Comments[0].CommentText.Length);

            for (int i = 0; i < commentsCount; i++)
            {
                embeddedBlog.Item.BlogComments[i].CommentText = comments[i].CommentText;
                embeddedBlog.Item.BlogComments[i].CommentedOn = DateTime.UtcNow;
            }

            var response = await this._embeddedOperations.UpdateBlog(embeddedBlog.Item);
            benchmark.BenchmarkResults.Add(new BenchmarkResult { EmbeddedBlogResponse = response });

            var result = await this._referentialOperations.GetOneBlogWithAllComments(referenceBlogId);
            var refernceBlog = result.Item1;
            var referenceComments = result.Item2;

            var referenceBlogBenchmarkResult = new BenchmarkResult { ReferentialBlogResponse = refernceBlog };

            var referenceCommentsGenerated = _blogGenerator.GenrateBlogComments(commentsCount, referenceComments[0].Item.CommentText.Length);
            
            for (int i = 0; i < commentsCount; i++)
            {
                referenceComments[i].Item.CommentText = referenceCommentsGenerated[i].CommentText;
                referenceComments[i].Item.CommentedOn = DateTime.UtcNow;
                var updateResponse = await this._referentialOperations.UpdateComment(referenceComments[i].Item);
                referenceBlogBenchmarkResult.ChildBenchmarkResults.Add(new BenchmarkResult { ReferentialCommentResponse = updateResponse, CommentGenerationResult = referenceCommentsGenerated[i] });
            }

            benchmark.AddResult(referenceBlogBenchmarkResult);

            return benchmark;
        }

        // Get all blogs
        public async Task<Benchmark> BenchmarkGettingAllBlogsWithoutComments()
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

            var response = await this._referentialOperations.GetAllBlogsWithAllComments();
            var referentialBlogs = response.Item1;
            var referentialComments = response.Item2;

            foreach (var blog in referentialBlogs)
            {
                var benchmarkResult = new BenchmarkResult { ReferentialBlogResponse = blog };

                foreach (var comment in referentialComments.Where(c => c.Item.BlogId == blog.Item.Id))
                {
                    benchmarkResult.ChildBenchmarkResults.Add(new BenchmarkResult { ReferentialCommentResponse = comment });
                }

                benchmark.BenchmarkResults.Add(benchmarkResult);
            }

            return benchmark;
        }

        // Get one blog
        public async Task<Benchmark> BenchmarkGettingBlog(string referenceBlogId, string embeddedBlogId)
        {
            var benchmark = new Benchmark(BenchmarkType.GetOneBlog);
            var embeddedBlog = await this._embeddedOperations.GetBlog(embeddedBlogId);
            benchmark.BenchmarkResults.Add(new BenchmarkResult { EmbeddedBlogResponse = embeddedBlog });
            var referentialBlog = await this._referentialOperations.GetBlog(referenceBlogId);
            benchmark.BenchmarkResults.Add(new BenchmarkResult { ReferentialBlogResponse = referentialBlog });
            return benchmark;
        }

        // Get one blogs with all comments
        public async Task<Benchmark> BenchmarkGettingBlogWithAllComments(string referenceBlogId, string embeddedBlogId)
        {
            var benchmark = new Benchmark(BenchmarkType.GetOneBlogWithAllComments);
            var embeddedBlog = await this._embeddedOperations.GetOneBlogWithAllComments(embeddedBlogId);
            benchmark.BenchmarkResults.Add(new BenchmarkResult { EmbeddedBlogResponse = embeddedBlog });
            var response = await this._referentialOperations.GetOneBlogWithAllComments(referenceBlogId);
            var referentialBlog = response.Item1;
            var commentResponses = response.Item2;
            benchmark.BenchmarkResults.Add(new BenchmarkResult
            {
                ReferentialBlogResponse = referentialBlog,
                ChildBenchmarkResults = commentResponses.Select(c => new BenchmarkResult { ReferentialCommentResponse = c }).ToList()
            });

            return benchmark;
        }

        // Get one blogs with n comments
        public async Task<Benchmark> BenchmarkGettingBlogWithSomeComments(string referenceBlogId, string embeddedBlogId, int numberOfComments)
        {
            var benchmark = new Benchmark(BenchmarkType.GetOneBlogWithSomeComments);
            var embeddedBlog = await this._embeddedOperations.GetOneBlogWithSomeComments(embeddedBlogId);
            benchmark.BenchmarkResults.Add(new BenchmarkResult { EmbeddedBlogResponse = embeddedBlog });

            var result = await this._referentialOperations.GetOneBlogWithSomeComments(referenceBlogId, numberOfComments);
            var blogResponse = result.Item1;
            var commentResponses = result.Item2;
            var referentialBlogBenchmarkResult = new BenchmarkResult
            {
                ReferentialBlogResponse = blogResponse,
                ChildBenchmarkResults = commentResponses.Select(c => new BenchmarkResult { ReferentialCommentResponse = c }).ToList()
            };

            benchmark.AddResult(referentialBlogBenchmarkResult);

            return benchmark;
        }
    }
}