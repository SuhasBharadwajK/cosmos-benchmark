using System.Collections.Generic;

namespace CosmosDbBenchmark.Models
{
    public class EmbeddedBlog : Blog
    {
        public EmbeddedBlog() : base(Constants.EmbeddedBlogTypeKey, BlogType.Embedded)
        {
        }

        public List<Comment> Comments 
        { 
            get
            {
                return this.BlogComments;
            }
            set
            {
                this.BlogComments = value;
            }
        }
    }
}