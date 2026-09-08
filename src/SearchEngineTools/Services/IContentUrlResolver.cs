using Umbraco.Cms.Core.Models;

namespace SearchEngineTools.Services
{
    /// <summary>
    /// Resolves absolute public URLs for Umbraco content.
    /// </summary>
    public interface IContentUrlResolver
    {
        /// <summary>
        /// Gets the absolute URL for the specified content item.
        /// </summary>
        /// <param name="content">The content item.</param>
        /// <returns>The absolute URL, or <c>null</c> when no published URL is available.</returns>
        public string? GetAbsoluteUrl(IContent content);

        /// <summary>
        /// Gets the absolute URLs for the specified content item and all published descendants.
        /// </summary>
        /// <param name="content">The root content item.</param>
        /// <returns>The absolute URLs for the content item and descendants.</returns>
        public IReadOnlyCollection<string> GetAbsoluteUrlsForDescendantsAndSelf(IContent content);
    }
}
