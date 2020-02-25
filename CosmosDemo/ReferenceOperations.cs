using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosDemo
{
    public class ReferenceOperations
    {
        private readonly CosmosDbRepository<Blog> _blogsRepository;
        private readonly CosmosDbRepository<Comment> _commentsRepository;

        public ReferenceOperations(CosmosDbRepository<Blog> blogsRepository, CosmosDbRepository<Comment> commentsRepository)
        {
            _blogsRepository = blogsRepository;
            _commentsRepository = commentsRepository;
        }

        public async Task<Blog> CreateBlog()
        {
            string blogId = Guid.NewGuid().ToString();

            Blog blog = new Blog
            {
                BlogId = blogId,
                Id = blogId,
                Type = "Post",
                Content = TextHelper.OneMBText,
                Comments = new List<string>()
            };
            Blog blogCreated = await _blogsRepository.AddAsync(blog);
           // Console.WriteLine(blogCreated);
            return blogCreated;
        }

        public async Task<Comment> CreateComment( Blog blog)
        {
            string commentId = Guid.NewGuid().ToString();
            Comment comment = new Comment
            {
                CommentId = commentId,
                Id = commentId,
                Details = TextHelper.OneMBText,
                BlogId = blog.BlogId,
                Likes = 40
            };
            blog.Comments.Add(commentId);
            await _commentsRepository.AddAsync(comment);
           // Console.WriteLine();
            UpdateBlog(blog);
            return comment;
        }

        public async void GetBlogWithComments(string id, string type)
        {
           // Console.WriteLine();
            await _blogsRepository.GetDocumentByIdAsync(id, type);
            string query = "SELECT * FROM c WHERE c.BlogId = '" + id + "'";
            await _commentsRepository.QueryItemsAsync(query);
        }

        public async void UpdateBlog(Blog blog)
        {
            blog.Content = TextHelper.OneMBText;
            // Console.WriteLine();
            await _blogsRepository.AddOrUpdateAsync(blog, blog.Type);
        }

        public async void UpdateComment(Comment comment)
        {
            comment.Details = TextHelper.OneMBText;
            //Console.WriteLine();
            await _commentsRepository.AddOrUpdateAsync(comment, comment.BlogId);
        }

        public async void RemoveBlog(Blog blog)
        {
            //Console.WriteLine();
        }

        public async void RemoveComment(Comment comment)
        {
            await _commentsRepository.RemoveAsync(comment.Id, comment.BlogId);
          //  Console.WriteLine();
        }

    }
}
