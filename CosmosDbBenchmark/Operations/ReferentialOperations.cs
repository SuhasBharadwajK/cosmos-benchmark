using System;
using System.Collections.Generic;
using CosmosDbBenchmark.Models;

namespace CosmosDbBenchmark
{
    public class ReferentialOperations
    {
        private CosmosDbRepository<Blog> referentialBlogRepository;

        public ReferentialOperations()
        {
            this.referentialBlogRepository = new CosmosDbRepository<Blog>();
        }

        public CosmosResponse<Blog> GetBlog(string blogId)
        {
            throw new NotImplementedException();
        }

        public List<CosmosResponse<Blog>> GetAllBlogs()
        {
            throw new NotImplementedException();
        }

        public List<CosmosResponse<Blog>> GetAllBlogsWithAllComments()
        {
            throw new NotImplementedException();
        }

        public List<CosmosResponse<Blog>> GetOneBlogWithAllComments()
        {
            throw new NotImplementedException();
        }

        public List<CosmosResponse<Blog>> GetAllBlogsWithSomeComments(int numberOfCommentsRequired)
        {
            throw new NotImplementedException();
        }

        public List<CosmosResponse<Blog>> GetOneBlogWithSomeComments(int numberOfCommentsRequired)
        {
            throw new NotImplementedException();
        }

        public CosmosResponse<Blog> CreateBlog(Blog blog)
        {
            throw new NotImplementedException();
        }

        public CosmosResponse<Blog> UpdateBlog(Blog blob)
        {
            throw new NotImplementedException();
        }

        CosmosResponse<ReferentialComment> AddComment(ReferentialComment comment)
        {
            throw new NotImplementedException();
        }

        CosmosResponse<ReferentialComment> UpdateComment(ReferentialComment comment)
        {
            throw new NotImplementedException();
        }
    }
}