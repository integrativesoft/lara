/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Integrative.Lara
{
    internal sealed class Connection
    {
        public Guid Id { get; }
        public IPAddress RemoteIp { get; }
        private readonly Dictionary<Guid, Document> _documents;

        public SessionStorage Storage { get; }
        public Session Session { get; }

        public AsyncEvent Closing { get; } = new AsyncEvent();

        public Connection(Guid id, IPAddress remoteId)
        {
            Id = id;
            RemoteIp = remoteId;
            _documents = new Dictionary<Guid, Document>();
            Storage = new SessionStorage();
            Session = new Session(this);
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

        public Document CreateDocument(IPage page, double keepAliveInterval)
        {
            var virtualId = Connections.CreateCryptographicallySecureGuid();
            var document = new Document(page, virtualId, keepAliveInterval);
            _documents.Add(virtualId, document);
            return document;
        }

        public async Task Discard(Guid documentId)
        {
            if (_documents.TryGetValue(documentId, out var document))
            {
                await document.NotifyUnload();
                // ReSharper disable once SuspiciousTypeConversion.Global
                if (document.Page is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                _documents.Remove(documentId);
            }
        }

        public IEnumerable<KeyValuePair<Guid, Document>> GetDocuments() => _documents;

        public bool IsEmpty => _documents.Count == 0;

        public async Task Close()
        {
            await Closing.InvokeAsync(this, new EventArgs());
            Session.Close();
        }
    }
}
