using System;

namespace CosmosDbBenchmark.Models
{
    public class ReferentialComment : DataEntity
    {
        public ReferentialComment() : base(Constants.CommentTypeKey)
        {
        }

        public string BlogId { get; set; }

        public string CommentText { get; set; }

        public string AuthorName { get; set; }

        public DateTime CommentedOn { get; set; }

        public override string EntityName => "Comment";
    }
}