using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.DeliveryApi;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Web.Common.Controllers;

namespace RazorBlog.Controllers.Api;

public class PostsController : UmbracoApiController
{
    private readonly IPublishedSnapshotAccessor _publishedSnapshotAccessor;
    private readonly IApiContentBuilder _apiContentBuilder;

    public PostsController(IPublishedSnapshotAccessor publishedSnapshotAccessor, IApiContentBuilder apiContentBuilder)
    {
        _publishedSnapshotAccessor = publishedSnapshotAccessor;
        _apiContentBuilder = apiContentBuilder;
    }

    // GET /umbraco/api/posts/byid
    [HttpGet]
    public IActionResult ById([FromQuery] Guid[] ids)
    {
        // NOTE: a similar "multiple content items by ID" API is slated for the Delivery API in a later version of Umbraco 12 
        var publishedContentCache = _publishedSnapshotAccessor.GetRequiredPublishedSnapshot().Content
                                    ?? throw new InvalidOperationException("Could not obtain the published content cache");

        var posts = ids.Select(publishedContentCache.GetById).WhereNotNull().ToArray();
        return Ok(new
        {
            posts = posts.Select(_apiContentBuilder.Build).ToArray()
        });
    }
}