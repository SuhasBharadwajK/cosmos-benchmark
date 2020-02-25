using System;
using System.Collections.Generic;

namespace CosmosDbBenchmark.Models
{
    public class Blog : DataEntity
    {
        public Blog() : base(Constants.BlogTypeKey)
        {
        }

        public Blog(string type, BlogType blogType) : base(type)
        {
            this.blogType = blogType;
        }

        public string name { get; set; }

        public string title { get; set; }

        public string content { get; set; }

        public DateTime createdOn { get; set; }
        
        public List<Comment> comments { get; set; }

        public BlogType blogType { get; set; }

        public override string entityName => (this.blogType == BlogType.Embedded ? "Embedded " : string.Empty) + "Blog";
    }
}