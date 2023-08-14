using System.Net.Http.Json;
using BlazorBlog.Models;
using Microsoft.AspNetCore.Components;

namespace BlazorBlog.Services;

public class ContentService : IContentService
{
    // TODO: read from appsettings
    private const string ApiBaseUrl = @"https://localhost:44304";

    private readonly HttpClient _httpClient;

    public ContentService(HttpClient httpClient) => _httpClient = httpClient;


    public async Task<Post[]> LatestPostsAsync()
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
                RequestUri = new Uri($"{ApiBaseUrl}/umbraco/delivery/api/v1/content/?fetch=children:/&expand=property:author&sort=updateDate:desc", UriKind.Absolute),
            Headers =
            {
                { "start-item", "posts" }
            }
        };
        var response = await _httpClient.SendAsync(request);
        var latestPostsData = await response.Content.ReadFromJsonAsync<QueryData<PostData>>();
        if (latestPostsData is null)
        {
            throw new ApplicationException("Could not fetch latest posts");
        }

        var posts = latestPostsData.Items.Select(ParsePost).ToArray();
        return posts;
    }

    public async Task<Post> PostAsync(string slug)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{ApiBaseUrl}/umbraco/delivery/api/v1/content/item/{slug}/?expand=property:author,latestPosts", UriKind.Absolute),
            Headers =
            {
                { "start-item", "posts" }
            }
        };
        var response = await _httpClient.SendAsync(request);
        var postData = await response.Content.ReadFromJsonAsync<PostData>();
        if (postData is null)
        {
            throw new ApplicationException("Could not fetch post");
        }
    
        return ParsePost(postData);
    }

    public async Task<Author> AuthorAsync(string slug)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{ApiBaseUrl}/umbraco/delivery/api/v1/content/item/{slug}/?expand=property:latestPosts", UriKind.Absolute),
            Headers =
            {
                { "start-item", "authors" }
            }
        };
        var response = await _httpClient.SendAsync(request);
        var authorData = await response.Content.ReadFromJsonAsync<AuthorData>();
        if (authorData is null)
        {
            throw new ApplicationException("Could not fetch author");
        }
    
        return ParseAuthor(authorData);
    }

    public async Task<Post[]> LatestPostsByTagAsync(string tag)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{ApiBaseUrl}/umbraco/delivery/api/v1/content/?fetch=children:/&expand=property:author&sort=updateDate:desc&filter=tag:{tag}", UriKind.Absolute),
            Headers =
            {
                { "start-item", "posts" }
            }
        };
        var response = await _httpClient.SendAsync(request);
        var latestPostsData = await response.Content.ReadFromJsonAsync<QueryData<PostData>>();
        if (latestPostsData is null)
        {
            throw new ApplicationException("Could not fetch latest posts");
        }

        var posts = latestPostsData.Items.Select(ParsePost).ToArray();
        return posts;
    }

    private Post ParsePost(PostData post) => new Post
    {
        Id = post.Id,
        Slug = post.Route.Path,
        Title = post.Name,
        CoverImage = ParsePicture(post.Properties.CoverImage),
        Date = post.UpdateDate,
        Author = ParseAuthor(post.Properties.Author),
        Excerpt = post.Properties.Excerpt,
        Content = (MarkupString)post.Properties.Content.Markup,
        Tags = post.Properties.Tags,
        LatestPosts = ParsePostSummaries(post.Properties.LatestPosts)
    };

    private PostSummary ParsePostSummary(PostSummaryData postSummary) => new PostSummary
    {
        Id = postSummary.Id,
        Slug = postSummary.Route.Path,
        Title = postSummary.Name,
        CoverImage = ParsePicture(postSummary.Properties.CoverImage),
        Date = postSummary.UpdateDate,
        Excerpt = postSummary.Properties.Excerpt,
        Tags = postSummary.Properties.Tags
    };
    
    private Picture ParsePicture(ImageData[] images)
    {
        var image = images.FirstOrDefault() ?? throw new ApplicationException("No images found");
        return new Picture
        {
            Url = $"{ApiBaseUrl}{image.Url}",
            Title = image.Name
        };
    }

    private Author ParseAuthor(AuthorData author) => new Author
    {
        Id = author.Id,
        Slug = author.Route.Path,
        Name = author.Name,
        Picture = ParsePicture(author.Properties.Picture),
        LatestPosts = ParsePostSummaries(author.Properties.LatestPosts)
    };

    private PostSummary[] ParsePostSummaries(PostSummaryData[]? postSummaries)
        => postSummaries?.Any() is true
            ? postSummaries.Select(ParsePostSummary).ToArray()
            : Array.Empty<PostSummary>();
    
    
    #region DTOs

    private class QueryData<TEntity> where TEntity : ContentEntityData
    {
        public int Total { get; init; }

        public required TEntity[] Items { get; init; }
    }

    private class PostData : PostDataEntity<PostPropertiesData>
    {
        
    }

    private class PostSummaryData : PostDataEntity<PostSummaryPropertiesData>
    {
        
    }

    private class PostDataEntity<T> : ContentEntityData
        where T : PostSummaryPropertiesData
    {
        public required T Properties { get; init; }
    }

    private class PostPropertiesData : PostSummaryPropertiesData
    {
        public required AuthorData Author { get; init; }

        public required RichTextData Content { get; init; }

        public PostSummaryData[]? LatestPosts { get; init; }
    }

    private class PostSummaryPropertiesData
    {
        public required ImageData[] CoverImage { get; init; }

        public required string Excerpt { get; init; }

        public required string[] Tags { get; init; }
    }

    private class AuthorData : ContentEntityData
    {
        public required AuthorPropertiesData Properties { get; init; }
    }

    private class AuthorPropertiesData
    {
        public required ImageData[] Picture { get; init; }
        
        public PostSummaryData[]? LatestPosts { get; init; }
    }
    
    private class ContentEntityData
    {
        public Guid Id { get; init; }

        public required string Name { get; init; }
        
        public required RouteData Route { get; init; }

        public required DateTime UpdateDate { get; init; }
    }
    
    private class RouteData
    {
        public required string Path { get; init; }
    }

    private class ImageData
    {
        public required string Name { get; init; }

        public required string Url { get; init; }
    }

    public class RichTextData
    {
        public required string Markup { get; init; }
    }
 
    #endregion
}