/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Integrative.Lara.Tools
{
    public static class ApplicationBuilderLaraExtensions
    {
        public static IApplicationBuilder UseLara(this IApplicationBuilder app, LaraOptions options)
        {
            if (options.AllowLocalhostOnly)
            {
                app.UseMiddleware<LocalhostFilter>();
            }
            app.UseMiddleware<LaraMiddleware>();
            if (options.ShowNotFoundPage)
            {
                app.UseMiddleware<NotFoundMiddleware>();
            }
            return app;
        }
    }
}
