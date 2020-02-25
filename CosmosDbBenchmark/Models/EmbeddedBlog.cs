namespace CosmosDbBenchmark.Models
{
    public class EmbeddedBlog : Blog
    {
        public EmbeddedBlog() : base(Constants.EmbeddedBlogTypeKey, BlogType.Embedded)
        {
        }
    }
}