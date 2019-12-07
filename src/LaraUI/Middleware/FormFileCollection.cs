/*
Copyright (c) 2019 Integrative Software LLC
Created: 12/2019
Author: Pablo Carbonell
*/

using Microsoft.AspNetCore.Http;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Integrative.Lara.Middleware
{
    [DataContract]
    class FormFileCollection : IFormFileCollection
    {
        [DataMember]
        public List<FormFile> InnerList { get; set; }

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

        public IFormFile this[string name] => InnerList.Find(x => x.Name == name);

        IFormFile IReadOnlyList<IFormFile>.this[int index] => InnerList[index];

        public IFormFile GetFile(string name)
        {
            return this[name];
        }

        public IReadOnlyList<IFormFile> GetFiles(string name)
        {
            return InnerList.FindAll(x => x.Name == name);
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
