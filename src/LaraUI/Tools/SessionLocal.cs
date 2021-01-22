/*
Copyright (c) 2019-2021 Integrative Software LLC
Created: 9/2019
Author: Pablo Carbonell
*/

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Integrative.Lara
{
    /// <summary>
    /// Represents ambient data that is local to a given session
    /// </summary>
    /// <typeparam name="T">Type of data to store</typeparam>
    public class SessionLocal<T>
    {
        private readonly Dictionary<Session, T> _storage;

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
        [MaybeNull]
        public T Value
        {
            get => GetValue();
            set => SetValue(value);
        }
        
        private T GetValue()
        {
            var session = GetSession();
            _storage.TryGetValue(session, out var value);
            return value;
        }

        private void SetValue([AllowNull] T value)
        {
            var session = GetSession();
            if (_storage.TryGetValue(session, out var previous))
            {
                if (LaraTools.SameValue(previous, value))
                {
                    return;
                }
                _storage.Remove(session);
                Store(value, session);
            }
            else
            {
                Store(value, session);
                session.CloseComplete += (_, _) => _storage.Remove(session);
            }
        }

        private void Store([AllowNull] T value, Session session)
        {
            if (value != null)
            {
                _storage.Add(session, value);
            }
        }

        private static Session GetSession()
        {
            if (LaraUI.Context is IPageContext page)
            {
                return page.Session;
            }

            if (LaraUI.Context is IWebServiceContext service
                && service.TryGetSession(out var session))
            {
                return session;
            }
            throw new NoCurrentSessionException(Resources.NoCurrentSession);
        }

    }
}
