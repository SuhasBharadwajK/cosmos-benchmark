namespace CosmosDbBenchmark.Models
{
    public class ReferentialBlog : Blog
    {
        public ReferentialBlog() : base(Constants.ReferentialBlogTypeKey, BlogType.Referential)
        {
        }
    }
}
