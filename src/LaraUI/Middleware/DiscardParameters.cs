/*
Copyright (c) 2019-2021 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;

namespace Integrative.Lara
{
    internal sealed class DiscardParameters
    {
        public Guid DocumentId { get; private set; }

        public static bool TryParse(HttpContext context, [NotNullWhen(true)] out DiscardParameters? parameters)
        {
            var query = context.Request.Query;
            return TryParse(query, out parameters);
        }

        public static bool TryParse(IQueryCollection query, [NotNullWhen(true)] out DiscardParameters? parameters)
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

            parameters = default;
            return false;
        }
    }
}
