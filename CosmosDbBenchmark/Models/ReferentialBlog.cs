using System;
using System.Collections.Generic;
using System.Text;

namespace CosmosDbBenchmark.Models
{
    public class ReferentialBlog : Blog
    {
        public ReferentialBlog() : base(Constants.ReferentialBlogTypeKey, BlogType.Referential)
        {
        }
    }
}
