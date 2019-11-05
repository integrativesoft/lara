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
        /// <param name="laraApp">Lara application</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseLara(this IApplicationBuilder app, Application laraApp, LaraOptions options)
        {
            app = app ?? throw new ArgumentNullException(nameof(app));
            laraApp = laraApp ?? throw new ArgumentNullException(nameof(laraApp));
            options = options ?? throw new ArgumentNullException(nameof(options));
            laraApp.CreateModeController(options.Mode);
            if (options.AllowLocalhostOnly || laraApp.AllowLocalhostOnly)
            {
                app.UseMiddleware<LocalhostFilter>();
            }
            if (options.AddWebSocketsMiddleware)
            {
                app.UseWebSockets();
            }
            app.UseMiddleware<LaraMiddleware>(laraApp, options);
            if (options.ShowNotFoundPage)
            {
                app.UseMiddleware<NotFoundMiddleware>(laraApp, options);
            }
            return app;
        }

        /// <summary>
        /// Use the Lara Web Engine.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseLara(this IApplicationBuilder app, LaraOptions options)
            => UseLara(app, LaraUI.DefaultApplication, options);

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
