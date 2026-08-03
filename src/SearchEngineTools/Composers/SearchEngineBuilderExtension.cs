using Microsoft.Extensions.DependencyInjection;
using SearchEngineTools.Services.Providers;
using Umbraco.Cms.Core.DependencyInjection;

namespace SearchEngineTools.Composers
{
    public static class SearchEngineBuilderExtension
    {
        /// <summary>
        /// Registers a search engine submission provider so that it is picked up by the submission queue worker
        /// </summary>
        /// <typeparam name="TProvider"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IUmbracoBuilder AddSearchEngineSubmissionProvider<TProvider>(this IUmbracoBuilder builder)
            where TProvider : class, ISearchEngineSubmissionProvider
        {
            builder.Services.AddScoped<ISearchEngineSubmissionProvider, TProvider>();

            return builder;
        }
    }
}
