using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CosmosDemo
{
    public class EmbeddedOperations
    {
      
        private readonly CosmosDbRepository<EmbeddedBlog> _blogsRepository;

        public EmbeddedOperations(CosmosDbRepository<EmbeddedBlog> blogsRepository)
        {
            _blogsRepository = blogsRepository;
        }

        public async Task<EmbeddedBlog> CreateBlog()
        {
            string blogId = Guid.NewGuid().ToString();
            EmbeddedBlog blog = new EmbeddedBlog
            {
                BlogId = blogId,
                Id = blogId,
                Type = "Post",
                Content = TextHelper.OneMBText,
                Comments = new List<Comment>()
            };
            //for(int i = 1; i < 11; i++)
            //{
            //    blog.Comments.Add(new Comment
            //    {
            //        CommentId = i.ToString(),
            //        Id = i.ToString(),
            //        Likes = i,
            //        Details = " this is comment "+ i
            //    });
            //}
            await _blogsRepository.AddAsync(blog);
           // Console.WriteLine();
            return blog;
        }

        public async void Get(string id, string type)
        {
            await _blogsRepository.GetDocumentByIdAsync(id, type);
          //  Console.WriteLine();
        }

        public async void QueryBlog(string id)
        {
            string query = "SELECT * FROM c WHERE c.BlogId = '" + id + "'";
            await _blogsRepository.QueryItemsAsync(query);
        }

        public async void Update(EmbeddedBlog blog)
        {
            string commentId = Guid.NewGuid().ToString();
            blog.Comments.Add(new Comment
            {
                CommentId = commentId,
                Id = commentId,
                Details = "never read a post like this",
                Likes = 1

        });
            await _blogsRepository.AddOrUpdateAsync(blog, blog.Type);
           // Console.WriteLine();
        }

        public async void UpdateComment(EmbeddedBlog blog)
        {
            blog.Comments[0].Details = "this is a updated comment";
            await _blogsRepository.AddOrUpdateAsync(blog, blog.Type);
           // Console.WriteLine();
        }

        public async void Remove(EmbeddedBlog blog)
        {
            await _blogsRepository.RemoveAsync(blog.Id, blog.Type);
           // Console.WriteLine();
        }

        public async void RemoveComment(EmbeddedBlog blog)
        {
            blog.Comments.Remove(blog.Comments[0]);
            await _blogsRepository.AddOrUpdateAsync(blog, blog.Type);
           // Console.WriteLine();
        }

    }
}
