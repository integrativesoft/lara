/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 11/2019
Author: Pablo Carbonell
*/

using Integrative.Lara;
using SampleProject.Components;
using System.Threading.Tasks;

namespace SampleProject.Pages
{
    internal class UploadFilePage : IPage
    {
        public Task OnGet()
        {
            LaraUI.Document.Body.AppendChild(new UploadSample());
            return Task.CompletedTask;
        }
    }
}
