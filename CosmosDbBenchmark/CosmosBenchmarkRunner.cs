using CosmosDbBenchmark.Generations;
using CosmosDbBenchmark.Models;
using System;
using System.Collections.Generic;
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

            var referentialBlogCount = 1;
            var embeddedBlogCount = 1;

            foreach (var result in blogGenerationResults)
            {
                var benchmarkResult = new BenchmarkResult { BlogGenerationResult = result };

                var blogCount = result.BlogType == BlogType.Embedded ? embeddedBlogCount : referentialBlogCount;

                var newBlog = new Blog
                {
                    Id = Guid.NewGuid().ToString(),
                    BlogType = result.BlogType,
                    Content = result.BlogText,
                    Title = Constants.BlogTitlePrefix + " " + blogCount,
                    Name = Constants.BlogNamePrefix + " " + blogCount,
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
                    newBlog.Type = Constants.EmbeddedBlogTypeKey;
                    benchmarkResult.EmbeddedBlogResponse = await this._embeddedOperations.CreateBlog(newBlog.CastTo<EmbeddedBlog>());
                    Console.WriteLine("Creating Embedded Blog No. " + embeddedBlogCount);
                    embeddedBlogCount++;
                }
                else
                {
                    newBlog.Type = Constants.BlogTypeKey;
                    Console.WriteLine("Creating Referential Blog No. " + referentialBlogCount);
                    var createBlogResult = await this._referentialOperations.CreateBlog(newBlog.CastTo<ReferentialBlog>(), referentialBlogCount);
                    benchmarkResult.ReferentialBlogResponse = createBlogResult.Item1;
                    benchmarkResult.ChildBenchmarkResults = createBlogResult.Item2.Select(c => new BenchmarkResult { BlogGenerationResult = result, ReferentialCommentResponse = c }).ToList();
                    referentialBlogCount++;
                }

                Console.WriteLine();

                benchmark.BenchmarkResults.Add(benchmarkResult);
            }

            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine();

            return benchmark;
        }

        // Update n blogs
        public async Task<Benchmark> BenchmarkUpdatingBlogs()
        {
            var benchmark = new Benchmark(BenchmarkType.UpdateAllBlogs);
            var blogGenerationResults = this._blogGenerator.GenerateBlogsWithComments();
            var embeddedBlogs = await this._embeddedOperations.GetAllBlogs();
            int embeddedIndex = 0;
            foreach (var embeddedBlog in embeddedBlogs)
            {
                embeddedBlog.Item.Content = blogGenerationResults[embeddedIndex].BlogText;
                var benchmarkResult = new BenchmarkResult { BlogGenerationResult = blogGenerationResults[embeddedIndex] };
                benchmarkResult.EmbeddedBlogResponse = await this._embeddedOperations.UpdateBlog(embeddedBlog.Item);
                benchmark.BenchmarkResults.Add(benchmarkResult);
                embeddedIndex++;
                Console.WriteLine("Updating Embedded Blog No. " + embeddedIndex);
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
                Console.WriteLine("Updating Referential Blog No. " + referentialIndex);
            }

            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine();

            return benchmark;
        }

        // Update n comments
        public async Task<Benchmark> BenchmarkUpdatingComments(string referenceBlogId, string embeddedBlogId, int commentsCount, List<BenchmarkResult> previousBenchmarkResults)
        {
            var benchmark = new Benchmark(BenchmarkType.UpdateAllCommentsInABlog);
            var embeddedBlog = await this._embeddedOperations.GetBlog(embeddedBlogId);

            var embeddedBlogGenerationResult = previousBenchmarkResults?.FirstOrDefault(r => r.EmbeddedBlogResponse != null && r.EmbeddedBlogResponse.Item.Id == embeddedBlogId).BlogGenerationResult;
            var referentialBlogGenerationResult = previousBenchmarkResults?.FirstOrDefault(r => r.ReferentialBlogResponse != null && r.ReferentialBlogResponse.Item.Id == referenceBlogId).BlogGenerationResult;

            benchmark.BenchmarkResults.Add(new BenchmarkResult { EmbeddedBlogResponse = embeddedBlog, BlogGenerationResult = embeddedBlogGenerationResult });
            var comments = _blogGenerator.GenrateBlogComments(commentsCount, embeddedBlog.Item.Comments[0].CommentText.Length);

            for (int i = 0; i < commentsCount; i++)
            {
                embeddedBlog.Item.BlogComments[i].CommentText = comments[i].CommentText;
                embeddedBlog.Item.BlogComments[i].CommentedOn = DateTime.UtcNow;
            }

            Console.WriteLine("Updating Comments in Embedded Blog with ID: " + embeddedBlogId);

            var response = await this._embeddedOperations.UpdateBlog(embeddedBlog.Item);
            benchmark.BenchmarkResults.Add(new BenchmarkResult { EmbeddedBlogResponse = response, BlogGenerationResult = embeddedBlogGenerationResult });

            var result = await this._referentialOperations.GetOneBlogWithAllComments(referenceBlogId);
            var refernceBlog = result.Item1;
            var referenceComments = result.Item2;

            var referenceBlogBenchmarkResult = new BenchmarkResult { ReferentialBlogResponse = refernceBlog, BlogGenerationResult = referentialBlogGenerationResult };

            var referenceCommentsGenerated = _blogGenerator.GenrateBlogComments(commentsCount, referenceComments[0].Item.CommentText.Length);
            
            Console.WriteLine("Updating Comments in Referential Blog with ID: " + referenceBlogId);
            for (int i = 0; i < commentsCount; i++)
            {
                referenceComments[i].Item.CommentText = referenceCommentsGenerated[i].CommentText;
                referenceComments[i].Item.CommentedOn = DateTime.UtcNow;
                var updateResponse = await this._referentialOperations.UpdateComment(referenceComments[i].Item);
                referenceBlogBenchmarkResult.ChildBenchmarkResults.Add(new BenchmarkResult { ReferentialCommentResponse = updateResponse, CommentGenerationResult = referenceCommentsGenerated[i], BlogGenerationResult = referentialBlogGenerationResult });
                Console.WriteLine("Updating Comment No. " + (i + 1) + " in Referential Blog with ID: " + referenceBlogId);
            }

            benchmark.AddResult(referenceBlogBenchmarkResult);

            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine();

            return benchmark;
        }

        // Get all blogs
        public async Task<Benchmark> BenchmarkGettingAllBlogsWithoutComments(List<BenchmarkResult> previousBenchmarkResults)
        {
            var benchmark = new Benchmark(BenchmarkType.GetAllBlogs);
            var embeddedBlogs = await this._embeddedOperations.GetAllBlogs();
            var embeddedBlogBenchmarkResults = previousBenchmarkResults?.Where(r => r.EmbeddedBlogResponse != null).ToList();
            var referentialBlogBenchmarkResults = previousBenchmarkResults?.Where(r => r.ReferentialBlogResponse != null).ToList();

            foreach (var blog in embeddedBlogs)
            {
                Console.WriteLine("Getting Embedded Blog with ID: " + blog.Item.Id);
                var benchmarkResult = new BenchmarkResult { EmbeddedBlogResponse = blog, BlogGenerationResult = embeddedBlogBenchmarkResults?.Where(r => r.EmbeddedBlogResponse.Item.Id == blog.Item.Id).FirstOrDefault()?.BlogGenerationResult };
                benchmark.BenchmarkResults.Add(benchmarkResult);
            }

            var referentialBlogs = await this._referentialOperations.GetAllBlogs();
            foreach (var blog in referentialBlogs)
            {
                Console.WriteLine("Getting Referential Blog with ID: " + blog.Item.Id);
                var benchmarkResult = new BenchmarkResult { ReferentialBlogResponse = blog, BlogGenerationResult = referentialBlogBenchmarkResults?.Where(r => r.ReferentialBlogResponse.Item.Id == blog.Item.Id).FirstOrDefault()?.BlogGenerationResult };
                benchmark.BenchmarkResults.Add(benchmarkResult);
            }

            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine();

            return benchmark;
        }

        // Get all blogs with all comments
        public async Task<Benchmark> BenchmarkGettingAllBlogsWithAllComments(List<BenchmarkResult> previousBenchmarkResults)
        {
            var benchmark = new Benchmark(BenchmarkType.GetAllBlogsWithAllComments);
            var embeddedBlogs = await this._embeddedOperations.GetAllBlogsWithAllComments();
            var embeddedBlogBenchmarkResults = previousBenchmarkResults?.Where(r => r.EmbeddedBlogResponse != null).ToList();
            var referentialBlogBenchmarkResults = previousBenchmarkResults?.Where(r => r.ReferentialBlogResponse != null).ToList();

            foreach (var blog in embeddedBlogs)
            {
                Console.WriteLine("Getting Embedded Blog with ID: " + blog.Item.Id);
                var benchmarkResult = new BenchmarkResult { EmbeddedBlogResponse = blog, BlogGenerationResult = embeddedBlogBenchmarkResults?.Where(r => r.EmbeddedBlogResponse.Item.Id == blog.Item.Id).FirstOrDefault()?.BlogGenerationResult };
                benchmark.BenchmarkResults.Add(benchmarkResult);
            }

            var response = await this._referentialOperations.GetAllBlogsWithAllComments();
            var referentialBlogs = response.Item1;
            var referentialComments = response.Item2;

            foreach (var blog in referentialBlogs)
            {
                Console.WriteLine("Getting Referential Blog with ID: " + blog.Item.Id);
                var referentialBlogBenchmarkResult = referentialBlogBenchmarkResults?.Where(r => r.ReferentialBlogResponse.Item.Id == blog.Item.Id).FirstOrDefault()?.BlogGenerationResult;
                var benchmarkResult = new BenchmarkResult { ReferentialBlogResponse = blog, BlogGenerationResult = referentialBlogBenchmarkResult };

                var count = 1;
                foreach (var comment in referentialComments.Where(c => c.Item.BlogId == blog.Item.Id))
                {
                    Console.WriteLine("Getting Comment " + count + " Blog with ID: " + blog.Item.Id);
                    benchmarkResult.ChildBenchmarkResults.Add(new BenchmarkResult { ReferentialCommentResponse = comment, BlogGenerationResult = referentialBlogBenchmarkResult });
                    count++;
                }

                benchmark.BenchmarkResults.Add(benchmarkResult);
            }

            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine();

            return benchmark;
        }

        // Get one blog
        public async Task<Benchmark> BenchmarkGettingBlog(string referenceBlogId, string embeddedBlogId, List<BenchmarkResult> previousBenchmarkResults)
        {
            var benchmark = new Benchmark(BenchmarkType.GetOneBlog);
            var embeddedBlog = await this._embeddedOperations.GetBlog(embeddedBlogId);

            var embeddedBlogGenerationResult = previousBenchmarkResults?.FirstOrDefault(r => r.EmbeddedBlogResponse != null && r.EmbeddedBlogResponse.Item.Id == embeddedBlogId).BlogGenerationResult;
            var referentialBlogGenerationResult = previousBenchmarkResults?.FirstOrDefault(r => r.ReferentialBlogResponse != null && r.ReferentialBlogResponse.Item.Id == referenceBlogId).BlogGenerationResult;

            Console.WriteLine("Getting Embedded Blog with ID: " + embeddedBlogId);
            benchmark.BenchmarkResults.Add(new BenchmarkResult { EmbeddedBlogResponse = embeddedBlog, BlogGenerationResult = embeddedBlogGenerationResult });
            var referentialBlog = await this._referentialOperations.GetBlog(referenceBlogId);
            Console.WriteLine("Getting Referential Blog with ID: " + referenceBlogId);
            benchmark.BenchmarkResults.Add(new BenchmarkResult { ReferentialBlogResponse = referentialBlog, BlogGenerationResult = referentialBlogGenerationResult });

            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine();

            return benchmark;
        }

        // Get one blogs with all comments
        public async Task<Benchmark> BenchmarkGettingBlogWithAllComments(string referenceBlogId, string embeddedBlogId, List<BenchmarkResult> previousBenchmarkResults)
        {
            var benchmark = new Benchmark(BenchmarkType.GetOneBlogWithAllComments);

            var embeddedBlogGenerationResult = previousBenchmarkResults?.FirstOrDefault(r => r.EmbeddedBlogResponse != null && r.EmbeddedBlogResponse.Item.Id == embeddedBlogId).BlogGenerationResult;
            var referentialBlogGenerationResult = previousBenchmarkResults?.FirstOrDefault(r => r.ReferentialBlogResponse != null && r.ReferentialBlogResponse.Item.Id == referenceBlogId).BlogGenerationResult;

            var embeddedBlog = await this._embeddedOperations.GetOneBlogWithAllComments(embeddedBlogId);

            Console.WriteLine("Getting Embedded Blog with ID: " + embeddedBlogId);

            benchmark.BenchmarkResults.Add(new BenchmarkResult { EmbeddedBlogResponse = embeddedBlog, BlogGenerationResult = embeddedBlogGenerationResult });

            var response = await this._referentialOperations.GetOneBlogWithAllComments(referenceBlogId);
            var referentialBlog = response.Item1;
            var commentResponses = response.Item2;

            Console.WriteLine("Getting Referential Blog with ID: " + referenceBlogId);

            benchmark.BenchmarkResults.Add(new BenchmarkResult
            {
                BlogGenerationResult = referentialBlogGenerationResult,
                ReferentialBlogResponse = referentialBlog,
                ChildBenchmarkResults = commentResponses.Select(c => new BenchmarkResult { ReferentialCommentResponse = c, BlogGenerationResult = referentialBlogGenerationResult }).ToList()
            });

            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine();

            return benchmark;
        }

        // Get one blogs with n comments
        public async Task<Benchmark> BenchmarkGettingBlogWithSomeComments(string referenceBlogId, string embeddedBlogId, int numberOfComments, List<BenchmarkResult> previousBenchmarkResults)
        {
            var benchmark = new Benchmark(BenchmarkType.GetOneBlogWithSomeComments);

            var embeddedBlogGenerationResult = previousBenchmarkResults?.FirstOrDefault(r => r.EmbeddedBlogResponse != null && r.EmbeddedBlogResponse.Item.Id == embeddedBlogId).BlogGenerationResult;
            var referentialBlogGenerationResult = previousBenchmarkResults?.FirstOrDefault(r => r.ReferentialBlogResponse != null && r.ReferentialBlogResponse.Item.Id == referenceBlogId).BlogGenerationResult;

            var embeddedBlog = await this._embeddedOperations.GetOneBlogWithSomeComments(embeddedBlogId);

            Console.WriteLine("Getting Embedded Blog with ID: " + embeddedBlogId);

            benchmark.BenchmarkResults.Add(new BenchmarkResult { EmbeddedBlogResponse = embeddedBlog, BlogGenerationResult = embeddedBlogGenerationResult });

            var result = await this._referentialOperations.GetOneBlogWithSomeComments(referenceBlogId, numberOfComments);
            var blogResponse = result.Item1;
            var commentResponses = result.Item2;

            Console.WriteLine("Getting Referential Blog with ID: " + referenceBlogId);

            var referentialBlogBenchmarkResult = new BenchmarkResult
            {
                BlogGenerationResult = referentialBlogGenerationResult,
                ReferentialBlogResponse = blogResponse,
                ChildBenchmarkResults = commentResponses.Select(c => new BenchmarkResult { ReferentialCommentResponse = c, BlogGenerationResult = referentialBlogGenerationResult }).ToList()
            };

            benchmark.AddResult(referentialBlogBenchmarkResult);

            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine();

            return benchmark;
        }

        public async Task DeleteAllItems()
        {
            await _embeddedOperations.CreateStoredProcedure();
            await _embeddedOperations.DropItemsFromContainer();
            await _referentialOperations.DropItemsFromContainer();
        }
    }
}