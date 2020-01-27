/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Xunit;

namespace Integrative.Lara.Tests.DOM
{
    public class ClassEditorTesting
    {
        [Fact]
        public void HasEmptyClassTrue()
        {
            Assert.True(ClassEditor.HasClass("", ""));
        }

        [Fact]
        public void EmptyClassFalse()
        {
            Assert.False(ClassEditor.HasClass("", "lele"));
        }

        [Fact]
        public void HasClassTrue()
        {
            Assert.True(ClassEditor.HasClass("aaa", "aaa"));
            Assert.True(ClassEditor.HasClass("aaa b", "aaa"));
            Assert.True(ClassEditor.HasClass("b aaa", "aaa"));
            Assert.True(ClassEditor.HasClass(" aaa", "aaa"));
            Assert.True(ClassEditor.HasClass("cc aaa ddd", "aaa"));
        }

        [Fact]
        public void RemoveClass()
        {
            Assert.Equal("lala", ClassEditor.RemoveClass("lala", ""));
            Assert.Equal("", ClassEditor.RemoveClass("lala", "lala"));
            Assert.Equal("", ClassEditor.RemoveClass(" lala ", "lala"));
            Assert.Equal("", ClassEditor.RemoveClass("lala", " lala "));
            Assert.Equal("blue", ClassEditor.RemoveClass("lala blue", "lala"));
            Assert.Equal("blue", ClassEditor.RemoveClass("blue lala", "lala"));
            Assert.Equal("red blue", ClassEditor.RemoveClass("red lala blue", "lala"));
            Assert.Equal("orange", ClassEditor.RemoveClass("orange", "lala"));
        }

        [Fact]
        public void AddClass()
        {
            Assert.Equal("lala", ClassEditor.AddClass("lala", "lala"));
            Assert.Equal("lala", ClassEditor.AddClass("  ", "lala"));
            Assert.Equal(" red lala", ClassEditor.AddClass(" red", "lala"));
        }
    }
}
