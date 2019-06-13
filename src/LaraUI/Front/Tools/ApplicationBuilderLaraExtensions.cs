/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Microsoft.AspNetCore.Builder;

namespace Integrative.Lara
{
    public static class ApplicationBuilderLaraExtensions
    {
        public static IApplicationBuilder UseLara(this IApplicationBuilder app, LaraOptions options)
        {
            if (options.AllowLocalhostOnly)
            {
                app.UseMiddleware<LocalhostFilter>();
            }
            if (options.AddWebSocketsMiddleware)
            {
                app.UseWebSockets();
            }
            app.UseMiddleware<LaraMiddleware>();
            if (options.ShowNotFoundPage)
            {
                app.UseMiddleware<NotFoundMiddleware>();
            }
            return app;
        }

        public static IApplicationBuilder UseLara(this IApplicationBuilder app)
        {
            return UseLara(app, new LaraOptions());
        }
    }
}
