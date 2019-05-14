/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Clara.Main;
using Microsoft.AspNetCore.Http;
using System;

namespace Integrative.Clara.Middleware
{
    sealed class DiscardParameters
    {
        public Guid DocumentId { get; set; }

        public static bool TryParse(HttpContext context, out DiscardParameters parameters)
        {
            var query = context.Request.Query;
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
