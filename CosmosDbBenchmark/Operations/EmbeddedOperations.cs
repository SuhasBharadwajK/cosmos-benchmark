using System;
using System.Collections.Generic;
using CosmosDbBenchmark.Models;

namespace CosmosDbBenchmark
{
    public class EmbeddedOperations
    {
        private CosmosDbRepository<EmbeddedBlog> blogsRepository;

        private CosmosDbRepository<ReferentialComment> commentsRepository;

        public EmbeddedOperations()
        {
            this.blogsRepository = new CosmosDbRepository<EmbeddedBlog>();
            this.commentsRepository = new CosmosDbRepository<ReferentialComment>();
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

        CosmosResponse<Blog> AddComment(Comment comment)
        {
            throw new NotImplementedException();
        }

        CosmosResponse<Blog> UpdateComment(Comment comment)
        {
            throw new NotImplementedException();
        }
    }
}