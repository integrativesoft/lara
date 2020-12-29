/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Microsoft.AspNetCore.Builder;
using System;
using System.ComponentModel;

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
        /// <returns>app in parameters</returns>
        public static IApplicationBuilder UseLara(this IApplicationBuilder app, Application laraApp, LaraOptions options)
        {
            app = app ?? throw new ArgumentNullException(nameof(app));
            laraApp = laraApp ?? throw new ArgumentNullException(nameof(laraApp));
            options = options ?? throw new ArgumentNullException(nameof(options));
            laraApp.CreateModeController(options.Mode);
            if (options.PublishAssembliesOnStart)
            {
                laraApp.PublishAssemblies();
            }
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
        /// <returns>app in parameters</returns>
        [Obsolete("Specify which Lara Application to use in the parameters of the call")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IApplicationBuilder UseLara(this IApplicationBuilder app, LaraOptions options)
            => UseLara(app, LaraUI.DefaultApplication, options);

        /// <summary>
        /// Use the Lara Web Engine.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <returns>app in parameters</returns>
        [Obsolete("Specify which Lara Application to use in the parameters of the call")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IApplicationBuilder UseLara(this IApplicationBuilder app)
        {
            return UseLara(app, new LaraOptions());
        }

        /// <summary>
        /// Use the Lara Web Engine
        /// </summary>
        /// <param name="app">ASP.NET Core ApplicationBuilder</param>
        /// <param name="laraApp">Lara Application</param>
        /// <returns></returns>
        public static IApplicationBuilder UseLara(this IApplicationBuilder app, Application laraApp)
        {
            return UseLara(app, laraApp, new LaraOptions());
        }

    }
}
