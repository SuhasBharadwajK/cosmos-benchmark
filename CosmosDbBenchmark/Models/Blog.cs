using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CosmosDbBenchmark.Models
{
    public class Blog : DataEntity
    {
        public Blog() : base(Constants.BlogTypeKey)
        {
        }

        public Blog(string type, BlogType blogType) : base(type)
        {
            this.BlogType = blogType;
        }

        public string Name { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime CreatedOn { get; set; }
        
        public BlogType BlogType { get; set; }

        [JsonIgnore]
        public List<Comment> BlogComments { get; set; }

        public override string EntityName => (this.BlogType == BlogType.Embedded ? "Embedded " : string.Empty) + "Blog";
    }
}