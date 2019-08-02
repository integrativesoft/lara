/*
Copyright (c) 2019 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Delta;
using Integrative.Lara.Main;
using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Integrative.Lara.Middleware
{
    class ServerEventsController
    {
        readonly Document _document;

        public ServerEventsController(Document document)
        {
            _document = document;
        }

        private bool _serverEventsEnabled;
        private WebSocket _serverEventsSocket;
        private TaskCompletionSource<bool> _completion;
        private bool _flushPending;

        public ServerEventsStatus ServerEventsStatus
        {
            get
            {
                if (!_serverEventsEnabled)
                {
                    return ServerEventsStatus.Disabled;
                }
                else if (_serverEventsSocket == null)
                {
                    return ServerEventsStatus.Connecting;
                }
                else
                {
                    return ServerEventsStatus.Enabled;
                }
            }
        }

        public void ServerEventsOn()
        {
            if (!_serverEventsEnabled)
            {
                _document.Enqueue(new ServerEventsDelta());
                _serverEventsEnabled = true;
            }
        }

        public Task ServerEventsOff()
        {
            _serverEventsEnabled = false;
            return DiscardSocket();
        }

        public Task NotifyUnload() => DiscardSocket();

        private async Task DiscardSocket()
        {
            if (_serverEventsSocket != null)
            {
                await PostEventHandler.CloseSocket(_serverEventsSocket);
                _serverEventsSocket = null;
                _completion.SetResult(true);
            }
        }

        public bool SocketRemainsOpen(string eventName)
        {
            return _serverEventsEnabled
                && eventName == GlobalConstants.ServerSideEvent;
        }

        public async Task ServerEventFlush()
        {
            if (PrepareFlush())
            {
                var json = _document.FlushQueue();
                await PostEventHandler.FlushMessage(_serverEventsSocket, json);
            }
        }

        private bool PrepareFlush()
        {
            if (!_serverEventsEnabled)
            {
                throw new InvalidOperationException("Server events are not enabled. Call ServerEventsOn() in order to use ServerEventFlush().");
            }
            else if (!_document.HasPendingChanges)
            {
                return false;
            }
            else if (_serverEventsSocket == null)
            {
                _flushPending = true;
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task GetSocketCompletion(WebSocket socket)
        {
            await DiscardSocket();
            _serverEventsSocket = socket;
            _completion = new TaskCompletionSource<bool>();
            await FlushIfPending();
            await _completion.Task;
        }

        private async Task FlushIfPending()
        {
            if (_flushPending)
            {
                _flushPending = false;
                await ServerEventFlush();
            }
        }

        public ServerEvent StartServerEvent()
        {
            return new ServerEvent(_document);
        }
    }
}
