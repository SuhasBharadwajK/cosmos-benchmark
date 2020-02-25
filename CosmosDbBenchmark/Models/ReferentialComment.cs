using System;

namespace CosmosDbBenchmark.Models
{
    public class ReferentialComment : DataEntity
    {
        public ReferentialComment() : base(Constants.CommentTypeKey)
        {
        }

        public string blogId { get; set; }

        public string commentText { get; set; }

        public string authorName { get; set; }

        public DateTime commentedOn { get; set; }

        public override string entityName => "Comment";
    }
}