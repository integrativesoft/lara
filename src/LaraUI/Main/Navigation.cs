/*
Copyright (c) 2019 Integrative Software LLC
Created: 6/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Delta;

namespace Integrative.Lara.Main
{
    sealed class Navigation : INavigation
    {
        readonly PageContext _context;

        public string RedirectLocation { get; private set; }

        public Navigation(PageContext context)
        {
            _context = context;
        }

        public void Replace(string location)
        {
            if (_context.Http.Request.Method == "GET")
            {
                ReplaceGet(location);
            }
            else
            {
                ReplacePost(location);
            }
        }

        private void ReplaceGet(string location)
        {
            RedirectLocation = location;
        }

        private void ReplacePost(string location)
        {
            ReplaceDelta.Enqueue(_context.Document, location);
        }
    }
}
