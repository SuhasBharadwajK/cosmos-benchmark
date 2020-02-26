using CosmosDbBenchmark.Models;
using System;
using System.Collections.Generic;

namespace CosmosDbBenchmark.Generations
{
    public class BlogGenerator
    {
        private int blogCount;

        private int commentsPerBlog;

        private int initialBlogSizeInKilobytes;

        private int blogSizeMultiplier;

        private int initialCommentSizeInBytes;

        private int commentSizeMultiplier;

        private int numberOfMultiplications;

        public BlogGenerator(
            int blogCount,
            int commentsPerBlog,
            int initialBlogSizeInKilobytes,
            int blogSizeMultiplier,
            int intialCommentSizeInBytes,
            int commentSizeMultiplier,
            int numberOfMultiplications)
        {
            this.blogCount = blogCount;
            this.initialCommentSizeInBytes = intialCommentSizeInBytes;
            this.commentsPerBlog = commentsPerBlog;
            this.commentSizeMultiplier = commentSizeMultiplier;
            this.initialBlogSizeInKilobytes = initialBlogSizeInKilobytes;
            this.blogSizeMultiplier = blogSizeMultiplier;
            this.numberOfMultiplications = numberOfMultiplications;
        }

        public List<BlogGenerationResult> GenerateBlogsWithComments()
        {
            var result = new List<BlogGenerationResult>();
            var blogSize = initialBlogSizeInKilobytes;
            var commentSize = initialCommentSizeInBytes;

            foreach (var enumValue in Enum.GetValues(typeof(BlogType)))
            {
                for (int i = 1; i <= numberOfMultiplications; i++)
                {
                    for (int j = 0; j < blogCount; j++)
                    {
                        var blogText = TextHelper.GetBytesOfString(blogSize, 1000);
                        var comments = TextHelper.GetStringsOfLength(commentsPerBlog, commentSize);

                        result.Add(new BlogGenerationResult
                        {
                            BlogSizeInKilobytes = blogSize,
                            CommentSizeInBytes = commentSize,
                            BlogText = blogText,
                            Comments = comments,
                            BlogType = (BlogType) enumValue
                        });
                    }

                    blogSize *= blogSizeMultiplier;
                    commentSize *= commentSizeMultiplier;
                }
            }

            return result;
        }
    }
}