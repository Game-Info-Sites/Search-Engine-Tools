using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace SearchEngineTools.Middleware
{
    public class IndexNowKeyVerificationStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                app.UseMiddleware<IndexNowKeyVerificationMiddleware>();
                next(app);
            };
        }
    }
}
