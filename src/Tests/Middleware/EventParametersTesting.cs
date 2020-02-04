/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 12/2019
Author: Pablo Carbonell
*/

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using Xunit;

namespace Integrative.Lara.Tests.Middleware
{
    public class EventParametersTesting
    {
        private readonly Guid _id = Guid.Parse("{2F97BE9D-1CC3-4EBD-BC93-EF748B774F1D}");

        [Fact]
        public void RoundTripMinimum()
        {
            var x = new EventParameters
            {
                DocumentId = _id,
                ElementId = "abc",
                EventName = "click",
                EventNumber = 1,
            };
            var json = LaraUI.JSON.Stringify(x);
            var result = LaraUI.JSON.Parse<EventParameters>(json);
            Assert.Equal(x.DocumentId, result.DocumentId);
            Assert.Equal(x.ElementId, result.ElementId);
            Assert.Equal(x.EventName, result.EventName);
            Assert.Equal(x.EventNumber, result.EventNumber);
        }

        [Fact]
        public void RoundTripEmptyFiles()
        {
            var x = new SocketEventParameters
            {
                DocumentId = _id,
                SocketFiles = new FormFileCollection()
            };
            var json = LaraUI.JSON.Stringify(x);
            var result = LaraUI.JSON.Parse<SocketEventParameters>(json);
            Assert.NotNull(result.SocketFiles);
            Assert.Empty(result.SocketFiles);
        }

        [Fact]
        public void RoundTripFile()
        {
            var x = new FormFile
            {
                Content = "hello",
                ContentDisposition = "a",
                ContentType = "b",
                FileName = "c",
                Name = "d"
            };
            var json = LaraUI.JSON.Stringify(x);
            var result = LaraUI.JSON.Parse<FormFile>(json);
            Assert.Equal(x.Content, result.Content);
            Assert.Equal(x.ContentDisposition, result.ContentDisposition);
            Assert.Equal(x.ContentType, result.ContentType);
            Assert.Equal(x.FileName, result.FileName);
            Assert.Equal(x.Length, result.Length);
            Assert.Equal(x.Name, result.Name);
        }

        [Fact]
        public void SerializeBytes()
        {
            var bytes = BuildBytes();
            var x = new FormFile
            {
                Content = Convert.ToBase64String(bytes)
            };
            var json = LaraUI.JSON.Stringify(x);
            var result = LaraUI.JSON.Parse<FormFile>(json);
            var output = Convert.FromBase64String(result.Content);
            Assert.Equal(256, bytes.Length);
            for (var index = 0; index < 256; index++)
            {
                Assert.Equal(index, output[index]);
            }
        }

        private static byte[] BuildBytes()
        {
            var bytes = new byte[256];
            for (var index = 0; index <= 255; index++)
            {
                bytes[index] = (byte)index;
            }
            return bytes;
        }

        [Fact]
        public void FileCollectionIterates()
        {
            var f1 = new FormFile();
            var f2 = new FormFile();
            var list = new FormFileCollection
            {
                InnerList = new List<FormFile>()
            };
            list.InnerList.Add(f1);
            list.InnerList.Add(f2);
            var other = new List<FormFile>();
            foreach (FormFile? f in list)
            {
                Assert.NotNull(f);
                other.Add(f!);
            }
            Assert.Equal(2, other.Count);
            Assert.Same(f1, other[0]);
            Assert.Same(f2, other[1]);
        }

        [Fact]
        public void NullCollectionEmpty()
        {
            var x = new FormFileCollection();
            var size = x.Count;
            Assert.Equal(0, size);
            x.InnerList = new List<FormFile>();
            size = x.Count;
            Assert.Equal(0, size);
        }

        [Fact]
        public void FindFileByName()
        {
            var f1 = new FormFile();
            var f2 = new FormFile
            {
                Name = "lala"
            };
            var x = new FormFileCollection
            {
                InnerList = new List<FormFile>
                {
                    f1,
                    f2
                }
            };
            var found = x.GetFile("lala");
            Assert.Same(f2, found);
        }

        [Fact]
        public void GetFileReadonlyList()
        {
            var f1 = new FormFile();
            var x = new FormFileCollection
            {
                InnerList = new List<FormFile>
                {
                    f1
                }
            };
            var list = x as IReadOnlyList<IFormFile>;
            var found = list[0];
            Assert.Same(f1, found);
        }

        [Fact]
        public void GetFilesByName()
        {
            var f1 = new FormFile
            {
                Name = "a"
            };
            var f2 = new FormFile
            {
                Name = "b"
            };
            var f3 = new FormFile
            {
                Name = "a"
            };
            var x = new FormFileCollection
            {
                InnerList = new List<FormFile>
                {
                    f1, f2, f3
                }
            };
            var found = x.GetFiles("a");
            Assert.Equal(2, found.Count);
            Assert.Same(f1, found[0]);
            Assert.Same(f3, found[1]);
        }

        [Fact]
        public void EnumrableInterface()
        {
            var x = new FormFileCollection();
            var y = (IEnumerable<IFormFile>) x;
            using var enumerator = y.GetEnumerator();
            Assert.False(enumerator.MoveNext());
        }
    }
}
