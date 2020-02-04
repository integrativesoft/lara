/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 12/2019
Author: Pablo Carbonell
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Http;

namespace Integrative.Lara
{
    [DataContract]
    internal class FormFileCollection : IFormFileCollection
    {
        [DataMember]
        public List<FormFile>? InnerList { get; set; }

        public int Count => GetCount();

        private int GetCount()
        {
            return InnerList?.Count ?? 0;
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
            return InnerList?.GetEnumerator() ?? GetEmptyEnumerator();
        }

        private static IEnumerator<IFormFile> GetEmptyEnumerator()
        {
            yield break;
        }
    }
}
