using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace SearchEngineTools.Services
{
    public class ContentUrlResolver(
        IUmbracoContextFactory umbracoContextFactory,
        IPublishedUrlProvider publishedUrlProvider
    ) : IContentUrlResolver
    {
        public string? GetAbsoluteUrl(IContent content)
        {
            using var contextRef = umbracoContextFactory.EnsureUmbracoContext();
            var publishedContent = contextRef.UmbracoContext.Content?.GetById(content.Id);

            return publishedContent?.Url(publishedUrlProvider, mode: UrlMode.Absolute);
        }

        public IReadOnlyCollection<string> GetAbsoluteUrlsForDescendantsAndSelf(IContent content)
        {
            using var contextRef = umbracoContextFactory.EnsureUmbracoContext();
            var publishedContent = contextRef.UmbracoContext.Content?.GetById(content.Id);

            if (publishedContent is null)
            {
                return Array.Empty<string>();
            }

            return publishedContent
                .DescendantsOrSelf()
                .Select(x => x.Url(publishedUrlProvider, mode: UrlMode.Absolute))
                .Where(IsValidUrl)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        private static bool IsValidUrl(string? url)
        {
            return !string.IsNullOrWhiteSpace(url) && url != "#";
        }
    }
}
