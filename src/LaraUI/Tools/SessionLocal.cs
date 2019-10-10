/*
Copyright (c) 2019 Integrative Software LLC
Created: 9/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Tools;
using System;
using System.Collections.Generic;

namespace Integrative.Lara
{
    /// <summary>
    /// Represents ambient data that is local to a given session
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SessionLocal<T>
    {
        readonly Dictionary<Session, T> _storage;

        /// <summary>
        /// Constructor
        /// </summary>
        public SessionLocal()
        {
            _storage = new Dictionary<Session, T>();
        }

        /// <summary>
        /// Value
        /// </summary>
        public T Value
        {
            get => GetValue();
            set => SetValue(value);
        }

        private T GetValue()
        {
            var session = GetSession();
            if (_storage.TryGetValue(session, out var value))
            {
                return value;
            }
            else
            {
                return default;
            }
        }

        private void SetValue(T value)
        {
            var session = GetSession();
            if (_storage.TryGetValue(session, out var previous))
            {
                if (LaraTools.SameValue(previous, value))
                {
                    return;
                }
                _storage.Remove(session);
                _storage.Add(session, value);
            }
            else
            {
                _storage.Add(session, value);
                session.AfterClose += (sender, args) => _storage.Remove(session);
            }
        }

        private static Session GetSession()
        {
            if (LaraUI.Page != null)
            {
                return LaraUI.Page.Session;
            }
            else if (LaraUI.Service != null && LaraUI.Service.TryGetSession(out var session))
            {
                return session;
            }
            else
            {
                throw new InvalidOperationException(Resources.NoCurrentSession);
            }            
        }

    }
}
