/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Microsoft.AspNetCore.Builder;
using System;

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
            app = app ?? throw new ArgumentNullException(nameof(app));
            options = options ?? throw new ArgumentNullException(nameof(options));
            if (options.AllowLocalhostOnly)
            {
                app.UseMiddleware<LocalhostFilter>();
            }
            if (options.AddWebSocketsMiddleware)
            {
                app.UseWebSockets();
            }
            app.UseMiddleware<LaraMiddleware>(options);
            if (options.ShowNotFoundPage)
            {
                app.UseMiddleware<NotFoundMiddleware>(options);
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
