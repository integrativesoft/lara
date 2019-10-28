/*
Copyright (c) 2019 Integrative Software LLC
Created: 10/2019
Author: Pablo Carbonell
*/

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Integrative.Lara.Middleware
{
    class Sequencer
    {
        readonly static Task<bool> TaskProceed = Task.FromResult(true);
        readonly static Task<bool> TaskAbort = Task.FromResult(false);

        readonly object _lock;
        readonly Dictionary<long, TaskCompletionSource<bool>> _pending;
        long _next;

        public Sequencer()
        {
            _lock = new object();
            _pending = new Dictionary<long, TaskCompletionSource<bool>>();
            _next = 1;
        }

        public Task<bool> WaitForTurn(long turnNumber)
        {
            if (turnNumber == 0)
            {
                return TaskProceed;
            }
            TaskCompletionSource<bool> completion = null;
            lock (_lock)
            {
                if (turnNumber == _next)
                {
                    _next++;
                    FlushPending();
                    return TaskProceed;
                }
                else if (turnNumber > _next)
                {
                    completion = new TaskCompletionSource<bool>();
                    _pending.Add(turnNumber, completion);
                }
                else
                {
                    return TaskAbort;
                }
            }
            return completion.Task;
        }

        public void AbortAll()
        {
            lock (_lock)
            {
                foreach (var item in _pending.Values)
                {
                    item.SetResult(false);
                }
                _pending.Clear();
            }
        }

        private void FlushPending()
        {
            while (_pending.TryGetValue(_next, out var source))
            {
                _pending.Remove(_next);
                source.SetResult(true);
                _next++;
            }
        }
    }
}
