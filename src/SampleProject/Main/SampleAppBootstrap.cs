/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara;
using System.Runtime.CompilerServices;

namespace SampleProject
{
    static class SampleAppBootstrap
    {
        public static void AppendTo(Element head)
        {
            head.AppendChild(new Link
            {
                Rel = "stylesheet",
                HRef = "https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css"
            });
            head.AppendChild(new Script
            {
                Src = "https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.7/umd/popper.min.js",
                Defer = true
            });
            head.AppendChild(new Script
            {
                Src = "https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/js/bootstrap.min.js",
                Defer = true
            });
        }
    }
}
