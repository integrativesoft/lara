/*
Copyright (c) 2019 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Integrative.Lara
{
    internal class ServerEventsController
    {
        private readonly Document _document;

        public ServerEventsController(Document document)
        {
            _document = document;
        }

        private bool _serverEventsEnabled;
        private WebSocket? _serverEventsSocket;
        private TaskCompletionSource<bool>? _completion;
        private bool _flushPending;

        public ServerEventsStatus ServerEventsStatus
            => CalculateServerEventsStatus(_serverEventsEnabled, _serverEventsSocket);

        internal static ServerEventsStatus CalculateServerEventsStatus(bool enabled, WebSocket? socket)
        {
            if (!enabled)
            {
                return ServerEventsStatus.Disabled;
            }
            else if (socket == null)
            {
                return ServerEventsStatus.Connecting;
            }
            else
            {
                return ServerEventsStatus.Enabled;
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

        internal async Task DiscardSocket()
        {
            if (_serverEventsSocket != null)
            {
                _completion?.SetResult(true);
                await PostEventHandler.CloseSocket(_serverEventsSocket);
                _serverEventsSocket = null;
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
                if (_serverEventsSocket != null)
                {
                    await PostEventHandler.FlushMessage(_serverEventsSocket, json);
                }
            }
        }

        private bool PrepareFlush()
        {
            if (!_serverEventsEnabled)
            {
                throw new InvalidOperationException(Resources.ServerEventsNotEnabled);
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

        public async Task<TaskCompletionSource<bool>> GetSocketCompletion(WebSocket socket)
        {
            await DiscardSocket();
            _serverEventsSocket = socket;
            _completion = new TaskCompletionSource<bool>();
            await FlushIfPending();
            return _completion;
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
