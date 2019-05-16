/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Main;
using Microsoft.AspNetCore.Http;
using System;

namespace Integrative.Lara.Middleware
{
    sealed class DiscardParameters
    {
        public Guid DocumentId { get; set; }

        public static bool TryParse(HttpContext context, out DiscardParameters parameters)
        {
            var query = context.Request.Query;
            return TryParse(query, out parameters);
        }

        public static bool TryParse(IQueryCollection query, out DiscardParameters parameters)
        {
            if (MiddlewareCommon.TryGetParameter(query, "doc", out var documentText)
                && Guid.TryParseExact(documentText, GlobalConstants.GuidFormat, out var documentId))
            {
                parameters = new DiscardParameters
                {
                    DocumentId = documentId,
                };
                return true;
            }
            else
            {
                parameters = default;
                return false;
            }
        }
    }
}
