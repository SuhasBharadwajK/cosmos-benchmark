using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CosmosDbBenchmark.Models;

namespace CosmosDbBenchmark
{
    public class ReferentialOperations
    {
        private CosmosDbRepository<Blog> referentialBlogRepository;
        private CosmosDbRepository<ReferentialComment> referentialCommentRepository;

        public ReferentialOperations()
        {
            this.referentialBlogRepository = new CosmosDbRepository<Blog>();
            this.referentialCommentRepository = new CosmosDbRepository<ReferentialComment>();
        }

        public async Task<CosmosResponse<Blog>> GetBlog(string blogId)
        {
           return await referentialBlogRepository.GetDocumentByIdAsync(blogId,Constants.BlogTypeKey);
        }

        public async Task<List<CosmosResponse<Blog>>> GetAllBlogs()
        {
            return await referentialBlogRepository.QueryItemsAsync("select * from c");
        }

        public async Task<List<CosmosResponse<Blog>>> GetAllBlogsWithAllComments()
        {
            List<CosmosResponse<Blog>> blogs = await referentialBlogRepository.QueryItemsAsync("select * from c");
            foreach(var blog in blogs)
            {
                List<CosmosResponse<ReferentialComment>> comments = await referentialCommentRepository.QueryItemsAsync("SELECT * FROM c WHERE c.BlogId = '" + blog.Item.Id + "'");
                foreach (var comment in comments)
                {
                    blog.RequestCharge += comment.RequestCharge;
                }
            }
            return blogs;
        }

        public async Task<CosmosResponse<Blog>> GetOneBlogWithAllComments(string blogId)
        {
            CosmosResponse<Blog> blog = await referentialBlogRepository.GetDocumentByIdAsync(blogId, Constants.BlogTypeKey);
            List<CosmosResponse<ReferentialComment>> comments = await referentialCommentRepository.QueryItemsAsync("SELECT * FROM c WHERE c.BlogId = '" + blog.Item.Id + "'");
            foreach(var comment in comments)
            {
                blog.RequestCharge += comment.RequestCharge;
            }
            return blog;
        }

        public async Task<List<CosmosResponse<Blog>>> GetAllBlogsWithSomeComments(int numberOfCommentsRequired)
        {
            List<CosmosResponse<Blog>> blogs = await referentialBlogRepository.QueryItemsAsync("select * from c");
            foreach (var blog in blogs)
            {
                List<CosmosResponse<ReferentialComment>> comments = await referentialCommentRepository.QueryItemsAsync("SELECT TOP "+numberOfCommentsRequired+" * FROM c WHERE c.BlogId = '" + blog.Item.Id + "'");
                foreach(var comment in comments)
                {
                    blog.RequestCharge += comment.RequestCharge;
                }
            }
            return blogs;
        }

        public async Task<CosmosResponse<Blog>> GetOneBlogWithSomeComments(string blogId,int numberOfCommentsRequired)
        {
            CosmosResponse<Blog> blog = await referentialBlogRepository.GetDocumentByIdAsync(blogId, Constants.BlogTypeKey);
            List<CosmosResponse<ReferentialComment>> comments = await referentialCommentRepository.QueryItemsAsync("SELECT "+numberOfCommentsRequired+"* FROM c WHERE c.BlogId = '" + blog.Item.Id + "'");
            foreach(var comment in comments)
            {
                blog.RequestCharge += comment.RequestCharge;
            }
            return blog;
        }

        public async Task<CosmosResponse<Blog>> CreateBlog(Blog blog)
        {
            return await referentialBlogRepository.AddAsync(blog);
        }

        public async Task<CosmosResponse<Blog>> UpdateBlog(Blog blog)
        {
            return await referentialBlogRepository.AddOrUpdateAsync(blog,Constants.BlogTypeKey);
        }

        public async Task<CosmosResponse<ReferentialComment>> AddComment(string blogId,ReferentialComment comment)
        {
            comment.BlogId = blogId;
            CosmosResponse<ReferentialComment> commentResponse =  await referentialCommentRepository.AddAsync(comment);
            CosmosResponse<Blog> blog = await referentialBlogRepository.GetDocumentByIdAsync(blogId, Constants.BlogTypeKey);
            blog.Item.Comments.Add(new Comment
            {
                AuthorName = comment.AuthorName,
                CommentedOn = comment.CommentedOn,
                CommentText = comment.CommentText
            });
            CosmosResponse<Blog> blogUpdateResponse = await referentialBlogRepository.AddOrUpdateAsync(blog.Item,Constants.BlogTypeKey);
            commentResponse.RequestCharge += blog.RequestCharge + blogUpdateResponse.RequestCharge;
            return commentResponse;
        }

        public async Task<CosmosResponse<ReferentialComment>> UpdateComment(ReferentialComment comment)
        {
            return await referentialCommentRepository.AddOrUpdateAsync(comment, Constants.CommentTypeKey);
        }
    }
}