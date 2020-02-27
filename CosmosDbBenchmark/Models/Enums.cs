namespace CosmosDbBenchmark.Models
{
    public enum ComsosDbOperation
    {
        Create,
        Read,
        Update,
        Delete,
        Query
    }

    public enum BlogType
    {
        Referential,
        Embedded
    }

    public enum BenchmarkType
    {
        CreateAllBlogs,
        UpdateAllBlogs,
        UpdateAllCommentsInABlog,
        GetAllBlogs,
        GetAllBlogsWithAllComments,
        GetOneBlog,
        GetOneBlogWithAllComments,
        GetOneBlogWithSomeComments
    }
}