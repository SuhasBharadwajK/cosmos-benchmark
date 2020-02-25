using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CosmosDemo
{
    public class Blog : BaseEntity
    {
        public string BlogId { get; set; }

        public string Content { get; set; }

        public List<string> Comments { get; set; }
        
        [JsonProperty(PropertyName ="type")]
        public string Type { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class Comment : BaseEntity
    {
        public string CommentId { get; set; }
        
        [JsonProperty(PropertyName = "blogId")]
        public string BlogId { get; set; }
        public string Details { get; set; }
        public int Likes { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class EmbeddedBlog : BaseEntity
    {
        public string BlogId { get; set; }

        public string Content { get; set; }

        public List<Comment> Comments { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

    }

}
