/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;

namespace Integrative.Lara
{
    public class DuplicateElementId : InvalidOperationException
    {
        public DuplicateElementId()
        {
        }

        public DuplicateElementId(string message)
            : base(message)
        {
        }

        public DuplicateElementId(string message, Exception inner)
            : base(message, inner)
        {
        }

        public static DuplicateElementId Create(string id)
        {
            string message = $"Duplicate element Id: {id}";
            return new DuplicateElementId(message);
        }
    }
}
