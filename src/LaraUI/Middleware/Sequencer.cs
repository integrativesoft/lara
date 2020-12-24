/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 10/2019
Author: Pablo Carbonell
*/

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Integrative.Lara
{
    internal class Sequencer
    {
        private static readonly Task<bool> _TaskProceed = Task.FromResult(true);
        private static readonly Task<bool> _TaskAbort = Task.FromResult(false);

        private readonly object _lock;
        private readonly Dictionary<long, TaskCompletionSource<bool>> _pending;
        private long _next;

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
                return _TaskProceed;
            }
            TaskCompletionSource<bool>? completion;
            lock (_lock)
            {
                if (turnNumber == _next)
                {
                    _next++;
                    FlushPending();
                    return _TaskProceed;
                }

                if (turnNumber > _next)
                {
                    completion = new TaskCompletionSource<bool>();
                    _pending.Add(turnNumber, completion);
                }
                else
                {
                    return _TaskAbort;
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
