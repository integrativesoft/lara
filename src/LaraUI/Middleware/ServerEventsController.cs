/*
Copyright (c) 2019-2020 Integrative Software LLC
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

            return socket == null ? ServerEventsStatus.Connecting : ServerEventsStatus.Enabled;
        }

        public void ServerEventsOn()
        {
            if (_serverEventsEnabled) return;
            _document.Enqueue(new ServerEventsDelta());
            _serverEventsEnabled = true;
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

            if (!_document.HasPendingChanges)
            {
                return false;
            }

            if (_serverEventsSocket != null) return true;
            _flushPending = true;
            return false;
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
