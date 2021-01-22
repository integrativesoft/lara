/*
Copyright (c) 2019-2021 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Tests.Middleware;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Integrative.Lara.Tests.DOM
{
    public class ElementAttributes : DummyContextTesting
    {
        private int _counter;

        [Fact]
        public void ElementProperties()
        {
            TestElement<HtmlAnchorElement>("a");
            TestElement<HtmlButtonElement>("button");
            TestElement<HtmlColGroupElement>("colgroup");
            TestElement<HtmlImageElement>("img");
            TestElement<HtmlInputElement>("input");
            TestElement<HtmlLabelElement>("label");
            TestElement<HtmlLinkElement>("link");
            TestElement<HtmlLiElement>("li");
            TestElement<HtmlMetaElement>("meta");
            TestElement<HtmlMeterElement>("meter");
            TestElement<HtmlOptionElement>("option");
            TestElement<HtmlOptionGroupElement>("optgroup");
            TestElement<HtmlOlElement>("ol");
            TestElement<HtmlScriptElement>("script");
            TestElement<HtmlSelectElement>("select");
            TestElement<HtmlTableElement>("table");
            TestElement<HtmlTableCellElement>("td");
            TestElement<HtmlTableHeaderElement>("th");
            TestElement<HtmlTextAreaElement>("textarea");
            Assert.NotNull(Document.CreateElement("tbody") as HtmlTableSectionElement);
            Assert.NotNull(Document.CreateElement("tfoot") as HtmlTableSectionElement);
            Assert.NotNull(Document.CreateElement("thead") as HtmlTableSectionElement);
        }

        [Fact]
        public void ElementNeedsTag()
        {
            DomOperationsTesting.Throws<ArgumentException>(() => Element.Create(""));
        }
        
        private void TestElement<T>(string tagName) where T : Element
        {
            var instance = Activator.CreateInstance(typeof(T)) as Element;
            Assert.NotNull(instance);
            Assert.Equal(tagName, instance!.TagName);
            TestProperties(instance);
        }

        private void TestProperties(Element instance)
        {
            var type = instance.GetType();
            foreach (var property in type.GetProperties())
            {
                if (property.SetMethod != null)
                {
                    TestProperty(instance, property);
                }
            }
        }

        private void TestProperty(Element instance, PropertyInfo property)
        {
            var type = property.PropertyType;
            if (!GetTestValue(type, out var value))
            {
                return;
            }
            property.SetValue(instance, value);
            var result = property.GetValue(instance);
            Assert.Equal(value, result);
        }

        private bool GetTestValue(Type type, [NotNullWhen(true)] out object? value)
        {
            _counter++;
            if (type == typeof(bool))
            {
                value = true;
            }
            else if (type == typeof(int))
            {
                value = _counter;
            }
            else if (type == typeof(string))
            {
                value = _counter.ToString();
            }
            else
            {
                value = null;
                return false;
            }
            return true;
        }

        [Fact]
        public void GetChildPositionNotFound()
        {
            var a = Element.Create("div");
            var b = Element.Create("div");
            var index = b.GetChildElementPosition(a);
            Assert.Equal(-1, index);
        }

        [Fact]
        public void ElementDescendsFromItself()
        {
            var x = Element.Create("div");
            Assert.True(x.DescendsFrom(x));
        }

        [Fact]
        public void SetIntAttribute()
        {
            var input = new HtmlInputElement
            {
                Height = 10
            };
            Assert.Equal(10, input.Height);
            input.Height = null;
            Assert.Null(input.Height);
        }

        [Fact]
        public async void ElementOnOptions()
        {
            var executed = false;
            var div = Element.Create("div");
            div.On(new EventSettings
            {
                EventName = "click",
                Handler = () =>
                {
                    executed = true;
                    return Task.CompletedTask;
                }
            });
            var context = new Mock<IPageContext>();
            LaraUI.InternalContext.Value = context.Object;
            await div.NotifyEvent("click");
            Assert.True(executed);
        }

        [Fact]
        public void RemoveClassRemovesClass()
        {
            var button = new HtmlButtonElement
            {
                Class = "red blue green"
            };
            button.RemoveClass("blue");
            Assert.Equal("red green", button.Class);
        }

        [Fact]
        public void AddClassAddsClass()
        {
            var button = new HtmlButtonElement();
            button.AddClass("red");
            Assert.Equal("red", button.Class);
        }

        [Fact]
        public void SetFlagAttributes()
        {
            var button = new HtmlButtonElement();
            button.SetFlagAttribute("hidden", true);
            Assert.True(button.Hidden);
        }

        [Fact]
        public void InputAttributes()
        {
            var input = new HtmlInputElement
            {
                MaxLength = 5,
                Size = 3,
                Width = 11
            };
            Assert.Equal(5, input.MaxLength);
            Assert.Equal(3, input.Size);
            Assert.Equal(11, input.Width);
        }

        [Fact]
        public void EncodeTextNode()
        {
            var n1 = new TextNode("&lt;");
            var n2 = new TextNode("&lt;", false);
            Assert.Equal("&amp;lt;", n1.Data);
            Assert.Equal("&lt;", n2.Data);
        }

        [Fact]
        public void ImageProperties()
        {
            var image = new HtmlImageElement
            {
                Height = "1",
                Width = "2"
            };
            Assert.Equal("1", image.Height);
            Assert.Equal("2", image.Width);
        }

        [Fact]
        public void OrderedListAttributes()
        {
            var ol = new HtmlOlElement
            {
                Start = 1
            };
            Assert.Equal(1, ol.Start);
        }

        [Fact]
        public void TextAreaProperties()
        {
            var x = new HtmlTextAreaElement
            {
                Cols = 1,
                MaxLength = 2,
                Rows = 3
            };
            Assert.Equal(1, x.Cols);
            Assert.Equal(2, x.MaxLength);
            Assert.Equal(3, x.Rows);
        }

        [Fact]
        public void NotifyValueTextArea()
        {
            var x = new HtmlTextAreaElement();
            x.NotifyValue(new ElementEventValue
            {
                Value = "lala"
            });
            Assert.Equal("lala", x.Value);
        }

        [Fact]
        public void TableHeaderProperties()
        {
            var x = new HtmlTableHeaderElement
            {
                ColSpan = 1,
                RowSpan = 2
            };
            Assert.Equal(1, x.ColSpan);
            Assert.Equal(2, x.RowSpan);
        }

        [Fact]
        public void EventSettingsAttributes()
        {
            var x = new EventSettings
            {
                LongRunning = true,
                BlockOptions = new BlockOptions
                {
                    BlockedElementId = "aaa",
                    ShowHtmlMessage = "baa",
                    ShowElementId = "xxx"
                }
            };
            Assert.True(x.Block);
            Assert.Equal("aaa", x.BlockOptions.BlockedElementId);
            Assert.Equal("baa", x.BlockOptions.ShowHtmlMessage);
            Assert.Equal("xxx", x.BlockOptions.ShowElementId);
            Assert.True(x.LongRunning);
        }

        [Fact]
        public void LaraOptionsProperties()
        {
            var x = new LaraOptions
            {
                AllowLocalhostOnly = true,
                ShowNotFoundPage = false,
                AddWebSocketsMiddleware = false,
                Mode = ApplicationMode.BrowserApp,
                PublishAssembliesOnStart = true
            };
            Assert.True(x.AllowLocalhostOnly);
            Assert.False(x.ShowNotFoundPage);
            Assert.False(x.AddWebSocketsMiddleware);
            Assert.Equal(ApplicationMode.BrowserApp, x.Mode);
            Assert.True(x.PublishAssembliesOnStart);
        }

        [Fact]
        public void DuplicateElementEmptyConstructor()
        {
            var instance = Activator.CreateInstance<DuplicateElementIdException>();
            Assert.NotNull(instance);
        }

        [Fact]
        public void DuplicateElementInner()
        {
            var inner = new InvalidOperationException("lala");
            var instance = new DuplicateElementIdException("lele", inner);
            Assert.Same(inner, instance.InnerException);
            Assert.Equal("lele", instance.Message);
        }

        [Fact]
        public void TableCellProperties()
        {
            var x = new HtmlTableCellElement
            {
                ColSpan = 1,
                RowSpan = 2
            };
            Assert.Equal(1, x.ColSpan);
            Assert.Equal(2, x.RowSpan);
        }

        [Fact]
        public void ColGroupProperties()
        {
            var x = new HtmlColGroupElement
            {
                Span = 1
            };
            Assert.Equal(1, x.Span);
        }

        [Fact]
        public void LoopSelectOptions()
        {
            var select = new HtmlSelectElement();
            var option1 = new HtmlOptionElement();
            var group = Element.Create("optgroup");
            var option2 = new HtmlOptionElement();
            group.AppendChild(option2);
            select.AppendChild(option1);
            select.AppendChild(group);
            Assert.Equal( new List<HtmlOptionElement>{ option1, option2 }, select.Options);
        }

        [Fact]
        public void SelectProperties()
        {
            var select = new HtmlSelectElement
            {
                Size = 3
            };
            Assert.Equal(3, select.Size);
        }

        [Fact]
        public void SelectNotifyValue()
        {
            var select = new HtmlSelectElement();
            select.NotifyValue(new ElementEventValue
            {
                Value = "lala"
            });
            Assert.Equal("lala", select.Value);
        }

        [Fact]
        public void SelectAddOption()
        {
            var x = new HtmlSelectElement();
            x.AddOption("myvalue", "this is the text");
            var option = x.Options.FirstOrDefault();
            Assert.NotNull(option);
            Assert.Equal("myvalue", option?.Value);
            var text = option?.Children.FirstOrDefault() as TextNode;
            Assert.NotNull(text);
            Assert.Equal("this is the text", text!.Data);
        }

        [Fact]
        public void OptionWithValueGetsSelected()
        {
            var select = new HtmlSelectElement
            {
                Value = "lolo"
            };
            var option = new HtmlOptionElement
            {
                Value = "lolo"
            };
            select.AppendChild(option);
            Assert.True(option.Selected);
        }

        [Fact]
        public void AddGroupWithSelectedOption()
        {
            var select = new HtmlSelectElement
            {
                Value = "lolo"
            };
            var option = new HtmlOptionElement
            {
                Value = "lolo"
            };
            var group = new HtmlOptionGroupElement();
            group.AppendChild(option);
            select.AppendChild(group);
            Assert.True(option.Selected);
        }

        [Fact]
        public void AddSelectedOptionInGroup()
        {
            var select = new HtmlSelectElement
            {
                Value = "lolo"
            };
            var option = new HtmlOptionElement
            {
                Value = "lolo"
            };
            var group = new HtmlOptionGroupElement();
            select.AppendChild(group);
            group.AppendChild(option);
            Assert.True(option.Selected);
        }

        [Fact]
        public void SelectValueChangeOnChildOptions()
        {
            var select = new HtmlSelectElement();
            var opt1 = new HtmlOptionElement
            {
                Value = "a"
            };
            var opt2 = new HtmlOptionElement
            {
                Value = "b"
            };
            var group = new HtmlOptionGroupElement();
            group.AppendChild(opt2);
            select.AppendChild(opt1);
            select.AppendChild(group);
            select.Value = "a";
            Assert.True(opt1.Selected);
            Assert.False(opt2.Selected);
            select.Multiple = true;
            select.Value = "b";
            Assert.True(opt1.Selected);
            Assert.True(opt2.Selected);
        }

        [Fact]
        public void MeterProperties()
        {
            var x = new HtmlMeterElement
            {
                High = 80,
                Low = 20,
                Max = 100,
                Min = 1,
                Optimum = 50,
                Value = 55
            };
            Assert.Equal(80, x.High);
            Assert.Equal(20, x.Low);
            Assert.Equal(100, x.Max);
            Assert.Equal(1, x.Min);
            Assert.Equal(50, x.Optimum);
            Assert.Equal(55, x.Value);
        }

        [Fact]
        public void ElementToStringSuffix()
        {
            var div = Element.Create("div");
            div.Id = "lolo";
            div.Class = "red";
            Assert.Equal("div #lolo red", div.ToString());
        }

        [Fact]
        public void IgnoreNotificationsNotFound()
        {
            var x = Element.Create("div");
            var task = x.NotifyEvent("lala");
            Assert.Same(Task.CompletedTask, task);
        }

        [Fact]
        public void AppendTextMergesNodes()
        {
            var x = Element.Create("div");
            x.AppendText("hi");
            x.AppendText(" ");
            x.AppendText("bye");
            Assert.Equal(1, x.ChildCount);
            var node = x.GetChildAt(0) as TextNode;
            Assert.NotNull(node);
            Assert.Equal("hi bye", node!.Data);
        }
    }
}
