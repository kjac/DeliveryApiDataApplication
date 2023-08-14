namespace BlazorBlog.Models;

public class PostSummary : ContentEntity
{
    public required string Title { get; init; }
    
    public required Picture CoverImage { get; init; }

    public required DateTime Date { get; init; }
    
    public required string Excerpt { get; init; }
    
    public required string[] Tags { get; init; }
}
