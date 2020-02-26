using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CosmosDbBenchmark.Models;

namespace CosmosDbBenchmark
{
    public class EmbeddedOperations
    {
        private CosmosDbRepository<EmbeddedBlog> blogsRepository;

        public EmbeddedOperations()
        {
            this.blogsRepository = new CosmosDbRepository<EmbeddedBlog>();
        }

        public async Task<CosmosResponse<EmbeddedBlog>> GetBlog(string blogId)
        {
            return await blogsRepository.GetDocumentByIdAsync(blogId,Constants.EmbeddedBlogTypeKey);
        }

        public async Task<List<CosmosResponse<EmbeddedBlog>>> GetAllBlogs()
        {
            return await blogsRepository.QueryItemsAsync("select * from c");
        }

        public async Task<List<CosmosResponse<EmbeddedBlog>>> GetAllBlogsWithAllComments()
        {
            return await blogsRepository.QueryItemsAsync("select * from c");
        }

        public async Task<CosmosResponse<EmbeddedBlog>> GetOneBlogWithAllComments(string blogId)
        {
           return await blogsRepository.GetDocumentByIdAsync(blogId, Constants.EmbeddedBlogTypeKey);
        }

        public async Task<List<CosmosResponse<EmbeddedBlog>>> GetAllBlogsWithSomeComments(string blogId)
        {
            return await blogsRepository.QueryItemsAsync("select * from c");
        }

        public async Task<CosmosResponse<EmbeddedBlog>> GetOneBlogWithSomeComments(string blogId,int numberOfCommentsRequired)
        {
            return await blogsRepository.GetDocumentByIdAsync(blogId, Constants.EmbeddedBlogTypeKey);
        }

        public async Task<CosmosResponse<EmbeddedBlog>> CreateBlog(EmbeddedBlog blog)
        {
            return await blogsRepository.AddAsync(blog);
        }

        public async Task<CosmosResponse<EmbeddedBlog>> UpdateBlog(EmbeddedBlog blog)
        {
            return await blogsRepository.AddOrUpdateAsync(blog,Constants.EmbeddedBlogTypeKey);
        }

        public async Task<CosmosResponse<EmbeddedBlog>> AddComment(string blogId,Comment comment)
        {
            CosmosResponse<EmbeddedBlog> blog = await blogsRepository.GetDocumentByIdAsync(blogId,Constants.EmbeddedBlogTypeKey);
            blog.Item.Comments.Add(comment);
            CosmosResponse<EmbeddedBlog> updatedBlog = await blogsRepository.AddOrUpdateAsync(blog.Item,Constants.EmbeddedBlogTypeKey);
            updatedBlog.RequestCharge += blog.RequestCharge;
            return updatedBlog;
        }

        public async Task<CosmosResponse<EmbeddedBlog>> UpdateComment(string blogId,Comment comment)
        {
            CosmosResponse<EmbeddedBlog> blog = await blogsRepository.GetDocumentByIdAsync(blogId, Constants.EmbeddedBlogTypeKey);
            blog.Item.Comments.Add(comment);
            CosmosResponse<EmbeddedBlog> updatedBlog = await blogsRepository.AddOrUpdateAsync(blog.Item, Constants.EmbeddedBlogTypeKey);
            updatedBlog.RequestCharge += blog.RequestCharge;
            return updatedBlog;
        }
    }
}