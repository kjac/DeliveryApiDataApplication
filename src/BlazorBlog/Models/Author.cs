namespace BlazorBlog.Models;

public class Author : ContentEntity
{
    public required string Name { get; init; }

    public required Picture Picture { get; init; }

    public required PostSummary[] LatestPosts { get; init; }
}
