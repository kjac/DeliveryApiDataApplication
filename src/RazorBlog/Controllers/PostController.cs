using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.DeliveryApi;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Services.OperationStatus;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace RazorBlog.Controllers;

public class PostController : RenderController
{
    private readonly ILogger<RenderController> _logger;
    private readonly IApiContentQueryService _apiContentQueryService;
    private readonly IPublishedSnapshotAccessor _publishedSnapshotAccessor;

    public PostController(
        ILogger<RenderController> logger,
        ICompositeViewEngine compositeViewEngine,
        IUmbracoContextAccessor umbracoContextAccessor,
        IApiContentQueryService apiContentQueryService,
        IPublishedSnapshotAccessor publishedSnapshotAccessor)
        : base(logger, compositeViewEngine, umbracoContextAccessor)
    {
        _logger = logger;
        _apiContentQueryService = apiContentQueryService;
        _publishedSnapshotAccessor = publishedSnapshotAccessor;
    }

    public override IActionResult Index()
    {
        if (CurrentPage is not Post post)
        {
            return base.Index();
        }

        // all posts are under the same root, so this works
        var postsRootKey = post.Parent!.Key;

        // author is a required field, so this works
        var authorKey = post.Author!.Key;

        // use the Delivery API query service to grab all posts by this author (applying our custom filter).
        // this may seem kinda silly; with ModelsBuilder models we can query the posts root in a strongly typed
        // manner for all posts by this author. however, if the dataset grows large (a whole lot of posts), that
        // strongly typed (in-memory) query might not be super performant.
        // even moreso, if we were querying for something that was expensive to calculate (unlike the author ID),
        // the in-memory query would quickly become sluggish and resource consuming. by using the Delivery API query
        // service, we're always leveraging pre-calculated (indexed) values for our query.
        var queryResult = _apiContentQueryService.ExecuteQuery(
            $"children:{postsRootKey}",
            new[] { $"author:{authorKey}" },
            new[] { "updateDate:desc" },
            0,
            10);

        if (queryResult.Success is false)
        {
            _logger.LogWarning(
                queryResult.Exception,
                "Could not execute the \"get posts by author\" query against the Delivery API index - the returned status was: {status}",
                queryResult.Status);

            return base.Index();
        }
        
        var publishedContentCache = _publishedSnapshotAccessor.GetRequiredPublishedSnapshot().Content
                                    ?? throw new InvalidOperationException("Could not obtain the published content cache");

        // NOTE: this is not really pretty. we should create a proper view model for the view instead of extending the ModelsBuilder one.
        //       for the sake of this demo, we'll make do with this quick-and-dirty fix.
        post.PostsByAuthor = queryResult.Result.Items.Select(publishedContentCache.GetById).WhereNotNull().OfType<Post>().ToArray();

        return base.Index();
    }
}