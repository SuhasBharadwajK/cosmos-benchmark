using Microsoft.Azure.Cosmos;
using MongoDB.Driver;
using System;
using System.Linq;

namespace CosmosDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
               // RunEmbeddedOperations();
                RunReferenceOperations();
            }
            catch (CosmosException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}", de.StatusCode, de);
            }
            catch (MongoException me)
            {
                Exception baseException = me.GetBaseException();
                Console.WriteLine("{0} error occurred", baseException);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
            }
            finally
            {
                Console.ReadKey();
            }
        }

        private async static void RunEmbeddedOperations()
        {
            Console.WriteLine("intializing cosmos db...");
            CosmosDbRepository<EmbeddedBlog> dbRepository = new CosmosDbRepository<EmbeddedBlog>("FamilyDatabase","EmbeddedBlogs");
            Console.WriteLine("connected");
            Console.WriteLine();
            EmbeddedOperations operations = new EmbeddedOperations(dbRepository);
            Console.WriteLine("creating new blog with comment");
            EmbeddedBlog blog = await operations.CreateBlog();
            Console.WriteLine("Fetching blog with ID..");
            operations.Get(blog.Id,blog.Type);
            Console.WriteLine("updating the blog");
            operations.Update(blog);
            Console.WriteLine("updating the comment");
            operations.UpdateComment(blog);
            Console.WriteLine("Fetching blog with query..");
            operations.QueryBlog(blog.Id);
            //Console.WriteLine("removing the blog");
            //operations.Remove(blog);
            //Console.WriteLine("removing the comment");
            //operations.RemoveComment(blog);
        }

        private async static void RunReferenceOperations()
        {
            Console.WriteLine("intializing Blogs Container....");
            CosmosDbRepository<Blog> blogRepository = new CosmosDbRepository<Blog>("FamilyDatabase","Blogs");
            Console.WriteLine("connected");
            Console.WriteLine();
            Console.WriteLine("intializing Comments Container....");
            CosmosDbRepository<Comment> commentRepository = new CosmosDbRepository<Comment>("FamilyDatabase", "Comments");
            Console.WriteLine("connected");
            Console.WriteLine();
            ReferenceOperations operations = new ReferenceOperations(blogRepository, commentRepository);
            Console.WriteLine("creating new blog");
            Blog blog = await operations.CreateBlog();
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("creating new comment for that blog");
            Comment comment = await operations.CreateComment(blog);
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("Fetching blog with comments..");
            operations.GetBlogWithComments(blog.Id,blog.Type);
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("updating the blog");
            operations.UpdateBlog(blog);
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("updating the comment");
            operations.UpdateComment(comment);
            Console.WriteLine("removing the comment");
            operations.RemoveComment(comment);
            Console.WriteLine("removing the blog");
            operations.RemoveBlog(blog);


        }




    }
}
