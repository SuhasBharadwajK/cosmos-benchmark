using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CosmosDbBenchmark.Models;

namespace CosmosDbBenchmark
{
    public class ReferentialOperations
    {
        private CosmosDbRepository<ReferentialBlog> referentialBlogRepository;
        private CosmosDbRepository<ReferentialComment> referentialCommentRepository;

        public ReferentialOperations()
        {
            this.referentialBlogRepository = new CosmosDbRepository<ReferentialBlog>();
            this.referentialCommentRepository = new CosmosDbRepository<ReferentialComment>();
        }

        public async Task<CosmosResponse<ReferentialBlog>> GetBlog(string blogId)
        {
            return await referentialBlogRepository.GetDocumentByIdAsync(blogId, Constants.BlogTypeKey);
        }

        public async Task<List<CosmosResponse<ReferentialBlog>>> GetAllBlogs()
        {
            return await referentialBlogRepository.QueryItemsAsync("select * from c");
        }

        public async Task<Tuple<List<CosmosResponse<ReferentialBlog>>, List<CosmosResponse<ReferentialComment>>>> GetAllBlogsWithAllComments()
        {
            List<CosmosResponse<ReferentialBlog>> blogs = await referentialBlogRepository.QueryItemsAsync("select * from c");
            List<CosmosResponse<ReferentialComment>> comments = new List<CosmosResponse<ReferentialComment>>();
            foreach (var blog in blogs)
            {
                comments = await referentialCommentRepository.QueryItemsAsync("SELECT * FROM c WHERE c.BlogId = '" + blog.Item.Id + "'");
                foreach (var comment in comments)
                {
                    blog.RequestCharge += comment.RequestCharge;
                }
            }
            return new Tuple<List<CosmosResponse<ReferentialBlog>>, List<CosmosResponse<ReferentialComment>>>(blogs,comments);
        }

        public async Task<Tuple<CosmosResponse<ReferentialBlog>, List<CosmosResponse<ReferentialComment>>>> GetOneBlogWithAllComments(string blogId)
        {
            CosmosResponse<ReferentialBlog> blog = await referentialBlogRepository.GetDocumentByIdAsync(blogId, Constants.BlogTypeKey);
            List<CosmosResponse<ReferentialComment>> comments = await referentialCommentRepository.QueryItemsAsync("SELECT * FROM c WHERE c.BlogId = '" + blog.Item.Id + "'");
            foreach (var comment in comments)
            {
                blog.RequestCharge += comment.RequestCharge;
            }
            return new Tuple<CosmosResponse<ReferentialBlog>, List<CosmosResponse<ReferentialComment>>>(blog, comments);

        }

        public async Task<List<CosmosResponse<ReferentialBlog>>> GetAllBlogsWithSomeComments(int numberOfCommentsRequired)
        {
            List<CosmosResponse<ReferentialBlog>> blogs = await referentialBlogRepository.QueryItemsAsync("select * from c");
            foreach (var blog in blogs)
            {
                List<CosmosResponse<ReferentialComment>> comments = await referentialCommentRepository.QueryItemsAsync("SELECT TOP " + numberOfCommentsRequired + " * FROM c WHERE c.BlogId = '" + blog.Item.Id + "'");
                foreach (var comment in comments)
                {
                    blog.RequestCharge += comment.RequestCharge;
                }
            }
            return blogs;
        }

        public async Task<Tuple<CosmosResponse<ReferentialBlog>, List<CosmosResponse<ReferentialComment>>>> GetOneBlogWithSomeComments(string blogId, int numberOfCommentsRequired)
        {
            CosmosResponse<ReferentialBlog> blog = await referentialBlogRepository.GetDocumentByIdAsync(blogId, Constants.BlogTypeKey);
            List<CosmosResponse<ReferentialComment>> comments = await referentialCommentRepository.QueryItemsAsync("SELECT TOP " + numberOfCommentsRequired + " * FROM c WHERE c.BlogId = '" + blog.Item.Id + "'");

            return new Tuple<CosmosResponse<ReferentialBlog>, List<CosmosResponse<ReferentialComment>>>(blog, comments);
        }

        public async Task<Tuple<CosmosResponse<ReferentialBlog>, List<CosmosResponse<ReferentialComment>>>> CreateBlog(ReferentialBlog blog)
        {
            CosmosResponse<ReferentialBlog> blogResponse = await referentialBlogRepository.AddAsync(blog);
            List<CosmosResponse<ReferentialComment>> commentRepsonses = new List<CosmosResponse<ReferentialComment>>();

            // Add comments from the blog object
            foreach (var comment in blogResponse.Item.BlogComments)
            {
                commentRepsonses.Add(await referentialCommentRepository.AddAsync(new ReferentialComment
                {
                    CommentedOn = comment.CommentedOn,
                    AuthorName = comment.AuthorName,
                    CommentText = comment.CommentText,
                    BlogId = blog.Id,
                    Id = Guid.NewGuid().ToString(),
                }));
            }

            return new Tuple<CosmosResponse<ReferentialBlog>, List<CosmosResponse<ReferentialComment>>>(blogResponse, commentRepsonses);
        }

        public async Task<CosmosResponse<ReferentialBlog>> UpdateBlog(Blog blog)
        {
            return await referentialBlogRepository.AddOrUpdateAsync((ReferentialBlog)blog, Constants.BlogTypeKey);
        }

        public async Task<CosmosResponse<ReferentialComment>> AddComment(string blogId, ReferentialComment comment)
        {
            comment.BlogId = blogId;
            CosmosResponse<ReferentialComment> commentResponse = await referentialCommentRepository.AddAsync(comment);
            return commentResponse;
        }

        public async Task<CosmosResponse<ReferentialComment>> UpdateComment(ReferentialComment comment)
        {
            return await referentialCommentRepository.AddOrUpdateAsync(comment, Constants.CommentTypeKey);
        }
    }
}