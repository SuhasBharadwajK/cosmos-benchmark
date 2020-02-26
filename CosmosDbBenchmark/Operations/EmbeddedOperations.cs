using System;
using System.Collections.Generic;
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

        public CosmosResponse<EmbeddedBlog> GetBlog(string blogId)
        {
            throw new NotImplementedException();
        }

        public List<CosmosResponse<EmbeddedBlog>> GetAllBlogs()
        {
            throw new NotImplementedException();
        }

        public List<CosmosResponse<EmbeddedBlog>> GetAllBlogsWithAllComments()
        {
            throw new NotImplementedException();
        }

        public List<CosmosResponse<EmbeddedBlog>> GetOneBlogWithAllComments()
        {
            throw new NotImplementedException();
        }

        public List<CosmosResponse<EmbeddedBlog>> GetAllBlogsWithSomeComments(int numberOfCommentsRequired)
        {
            throw new NotImplementedException();
        }

        public List<CosmosResponse<EmbeddedBlog>> GetOneBlogWithSomeComments(int numberOfCommentsRequired)
        {
            throw new NotImplementedException();
        }

        public CosmosResponse<EmbeddedBlog> CreateBlog(Blog blog)
        {
            throw new NotImplementedException();
        }

        public CosmosResponse<EmbeddedBlog> UpdateBlog(Blog blob)
        {
            throw new NotImplementedException();
        }

        CosmosResponse<EmbeddedBlog> AddComment(Comment comment)
        {
            throw new NotImplementedException();
        }

        CosmosResponse<EmbeddedBlog> UpdateComment(Comment comment)
        {
            throw new NotImplementedException();
        }
    }
}