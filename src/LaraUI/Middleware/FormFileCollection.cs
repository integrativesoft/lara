/*
Copyright (c) 2019 Integrative Software LLC
Created: 12/2019
Author: Pablo Carbonell
*/

using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Integrative.Lara.Middleware
{
    [DataContract]
    class FormFileCollection : IFormFileCollection
    {
        [DataMember]
        public List<FormFile>? InnerList { get; set; }

        public int Count => GetCount();

        private int GetCount()
        {
            if (InnerList == null)
            {
                return 0;
            }
            else
            {
                return InnerList.Count;
            }
        }

        public IFormFile this[string name] => GetInnerList().Find(x => x.Name == name);

        IFormFile IReadOnlyList<IFormFile>.this[int index] => GetInnerList()[index];

        private List<FormFile> GetInnerList()
        {
            return InnerList ?? throw new MissingMemberException(nameof(FormFileCollection), nameof(InnerList));
        }

        public IFormFile GetFile(string name)
        {
            return this[name];
        }

        public IReadOnlyList<IFormFile> GetFiles(string name)
        {
            return GetInnerList().FindAll(x => x.Name == name);
        }

        IEnumerator<IFormFile> IEnumerable<IFormFile>.GetEnumerator()
        {
            return GetEnumeratorInternal();
        }

        public IEnumerator GetEnumerator()
        {
            return GetEnumeratorInternal();
        }

        private IEnumerator<IFormFile> GetEnumeratorInternal()
        {
            if (InnerList == null)
            {
                return GetEmptyEnumerator();
            }
            else
            {
                return InnerList.GetEnumerator();
            }
        }

        private static IEnumerator<IFormFile> GetEmptyEnumerator()
        {
            yield break;
        }
    }
}
