/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Clara.DOM;
using System;
using System.Collections.Generic;
using System.Net;

namespace Integrative.Clara.Main
{
    sealed class Connection
    {
        public Guid Id { get; }
        public IPAddress RemoteId { get; }
        readonly Dictionary<Guid, Document> _documents;

        public Connection(Guid id, IPAddress remoteId)
        {
            Id = id;
            RemoteId = remoteId;
            _documents = new Dictionary<Guid, Document>();
        }

        public bool TryGetDocument(Guid virtualId, out Document document)
        {
            if (_documents.TryGetValue(virtualId, out document))
            {
                document.UpdateTimestamp();
                return true;
            }
            else
            {
                return false;
            }
        }

        public Document CreateDocument(BasePage page)
        {
            var document = new Document(page);
            var virtualId = Connections.CreateCryptographicallySecureGuid();
            _documents.Add(virtualId, document);
            FillTemplate(document, virtualId);
            document.UpdateTimestamp();
            return document;
        }

        private void FillTemplate(Document document, Guid virtualId)
        {
            var builder = new TemplateBuilder(document, virtualId);
            builder.Build();
        }

        public void Discard(Guid documentId)
        {
            if (_documents.TryGetValue(documentId, out var document))
            {
                if (document.Page is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                _documents.Remove(documentId);
            }
        }

        public IEnumerable<KeyValuePair<Guid, Document>> GetDocuments() => _documents;

        public bool IsEmpty => _documents.Count == 0;
    }
}
