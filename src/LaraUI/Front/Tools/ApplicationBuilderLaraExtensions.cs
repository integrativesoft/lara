/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Microsoft.AspNetCore.Builder;

namespace Integrative.Lara
{
    /// <summary>
    /// ASP.NET Core Extensions for using Lara
    /// </summary>
    public static class ApplicationBuilderLaraExtensions
    {
        /// <summary>
        /// Use the Lara Web Engine.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Use the Lara Web Engine.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseLara(this IApplicationBuilder app)
        {
            return UseLara(app, new LaraOptions());
        }
    }
}
