/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Tests.Middleware;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xunit;

namespace Integrative.Lara.Tests.DOM
{
    internal class MyInputData : BindableBase
    {
        private string? _myvalue;
        public string? MyValue
        {
            get => _myvalue;
            set => SetProperty(ref _myvalue, value);
        }

        private bool _mychecked;
        public bool MyChecked
        {
            get => _mychecked;
            set => SetProperty(ref _mychecked, value);
        }
    }

    public class BindingsTesting : DummyContextTesting
    {
        [Fact]
        public void SetInnerTextSetsText()
        {
            var x = Element.Create("span");
            x.InnerText = "hello";
            Assert.Equal(1, x.ChildCount);
            VerifyInnerData(x, "hello");
        }

        private static void VerifyInnerData(Element element, string data)
        {
            var node = element.GetChildAt(0) as TextNode;
            Assert.NotNull(node);
            Assert.Equal(data, node!.Data);
        }

        [Fact]
        public void InnerTextReplacesPrevious()
        {
            const string bye = "<bye";
            var x = Element.Create("span");
            x.InnerText = "hello";
            x.InnerText = bye;
            Assert.Equal(bye, x.InnerText);
            VerifyInnerData(x, "&lt;bye");
        }

        [Fact]
        [Obsolete("old methods")]
        public void BindInnerTextUpdates()
        {
            var data = new MyData();
            var div = Element.Create("div");
            div.BindInnerText(new BindInnerTextOptions<MyData>
            {
                BindObject = data,
                Property = x => x.Counter.ToString()
            });
            data.Counter = 5;
            VerifyInnerData(div, "5");
        }

        [Fact]
        [Obsolete("old methods")]
        public void BindGenericExecutes()
        {
            var data = new MyData();
            var div = Element.Create("div");
            div.Bind(new BindHandlerOptions<MyData>
            {
                BindObject = data,
                ModifiedHandler = (_, _) => div.InnerText = data.Counter.ToString()
            });
            data.Counter = 5;
            VerifyInnerData(div, "5");
        }

        [Fact]
        [Obsolete("old methods")]
        public void BindActionExecutes()
        {
            var data = new MyData();
            var div = Element.Create("div");
            div.Bind(data, _ => div.InnerText = data.Counter.ToString());
            data.Counter = 5;
            VerifyInnerData(div, "5");
        }

        [Fact]
        [Obsolete("old methods")]
        public void BindAttributeExecutes()
        {
            var data = new MyData();
            var div = Element.Create("div");
            div.BindAttribute(new BindAttributeOptions<MyData>
            {
                Attribute = "data-counter",
                BindObject = data,
                Property = x => x.Counter.ToString()
            });
            data.Counter = 5;
            Assert.Equal("5", div.GetAttribute("data-counter"));
        }

        [Fact]
        [Obsolete("old methods")]
        public void BindChildrenUpdates()
        {
            var collection = new ObservableCollection<MyData>
            {
                new MyData()
            };
            var span = Element.Create("span");
            span.BindChildren(new BindChildrenOptions<MyData>(collection, MyCreateCallback));
            collection.Add(new MyData());
            Assert.Equal(2, span.ChildCount);
        }

        [Fact]
        [Obsolete("old methods")]
        public void UnbindAllUnbinds()
        {
            var collection = new ObservableCollection<MyData>();
            var data = new MyData();
            var div = Element.Create("div");
            var span1 = Element.Create("span");
            var span2 = Element.Create("span");
            span2.Bind(data, _ => span2.InnerText = data.Counter.ToString());
            div.BindAttribute(new BindAttributeOptions<MyData>
            {
                Attribute = "data-counter",
                BindObject = data,
                Property = x => x.Counter.ToString()
            });
            div.BindChildren(new BindChildrenOptions<MyData>(collection, _ => Element.Create("div")));
            span1.BindInnerText(new BindInnerTextOptions<MyData>
            {
                BindObject = data,
                Property = x => x.Counter.ToString()
            });
            data.Counter = 5;
            div.UnbindAll();
            span1.UnbindAll();
            span2.UnbindAll();
            collection.Add(data);
            data.Counter = 10;
            VerifyInnerData(span1, "5");
            VerifyInnerData(span2, "5");
            Assert.Equal(0, div.ChildCount);
            Assert.Equal("5", div.GetAttribute("data-counter"));
        }

        [Fact]
        [Obsolete("old methods")]
        public void UnbindAtributeRuns()
        {
            var x = Element.Create("div");
            var data = new MyData();
            x.BindAttribute(new BindAttributeOptions<MyData>
            {
                Attribute = "data-counter",
                BindObject = data,
                Property = y => y.Counter.ToString()
            });
            data.Counter = 5;
            x.UnbindAll();
            data.Counter = 10;
            Assert.Equal("5", x.GetAttribute("data-counter"));
        }

        [Fact]
        [Obsolete("old methods")]
        public void UnbindAttributeRemovesAllAttributes()
        {
            var x = Element.Create("div");
            var data = new MyData();
            x.BindAttribute(new BindAttributeOptions<MyData>
            {
                Attribute = "data-counter",
                BindObject = data,
                Property = y => y.Counter.ToString()
            });
            x.BindAttribute(new BindAttributeOptions<MyData>
            {
                Attribute = "data-counter2",
                BindObject = data,
                Property = y => y.Counter.ToString()
            });
            data.Counter = 5;
            x.UnbindAll();
            data.Counter = 10;
            Assert.Equal("5", x.GetAttribute("data-counter"));
            Assert.Equal("5", x.GetAttribute("data-counter2"));
        }

        [Fact]
        [Obsolete("old methods")]
        public void UnbindInnerTextWorks()
        {
            var div = Element.Create("div");
            var data = new MyData
            {
                Counter = 5
            };
            div.BindInnerText(new BindInnerTextOptions<MyData>
            {
                BindObject = data,
                Property = x => x.Counter.ToString()
            });
            VerifyInnerData(div, "5");
            div.UnbindAll();
            data.Counter = 10;
            VerifyInnerData(div, "5");
        }

        [Fact]
        [Obsolete("old methods")]
        public void UnbindHandlerWorks()
        {
            var div = Element.Create("div");
            var data = new MyData();
            div.Bind(data, _ => div.InnerText = data.Counter.ToString());
            data.Counter = 3;
            div.UnbindAll();
            data.Counter = 8;
            VerifyInnerData(div, "3");
        }

        [Fact]
        [Obsolete("old methods")]
        public void UnbindChildrenWorks()
        {
            var collection = new ObservableCollection<MyData>();
            var div = Element.Create("div");
            div.BindChildren(new BindChildrenOptions<MyData>(collection, _ => Element.Create("span")));
            collection.Add(new MyData());
            div.UnbindAll();
            collection.Clear();
            Assert.NotEmpty(div.Children);
        }

        [Fact]
        [Obsolete("old methods")]
        public void GenericBindingDetectsCycles()
        {
            var div = Element.Create("div");
            var data = new MyData();
            div.BindAttribute(new BindAttributeOptions<MyData>
            {
                Attribute = "data-counter",
                BindObject = data,
                Property = x => x.Counter.ToString()
            });
            div.Bind(new BindHandlerOptions<MyData>
            {
                BindObject = data,
                ModifiedHandler = (_, _) => data.Counter++
            });
            var found = false;
            try
            {
                data.Counter = 3;
            }
            catch (InvalidOperationException)
            {
                found = true;
            }
            Assert.True(found);
        }

        [Obsolete("old methods")]
        private static Element MyCreateCallback(MyData arg)
        {
            var span = Element.Create("span");
            span.BindAttribute(new BindAttributeOptions<MyData>
            {
                BindObject = arg,
                Attribute = "data-counter",
                Property = _ => arg.Counter.ToString()
            });
            return span;
        }

        private class MyData : BindableBase
        {
            private int _counter;

            public MyData()
            {
            }

            public MyData(int counter)
            {
                Counter = counter;
            }

            public int Counter
            {
                get => _counter;
                set => SetProperty(ref _counter, value);
            }

            public bool IsEven => (_counter % 2) == 0;

            public override string ToString() => Counter.ToString();
        }

        [Fact]
        public void BindableBaseSkipsUnncesaryEvents()
        {
            var raised = false;
            var data = new MyData();
            data.PropertyChanged += (_, _) => raised = true;
            data.Counter = 0;
            Assert.False(raised);
        }

        [Fact]
        [Obsolete("old methods")]
        public void CollectionUpdaterMove()
        {
            var collection = new ObservableCollection<MyData>();
            var div = Element.Create("div");
            div.BindChildren(new BindChildrenOptions<MyData>(collection, MyCreateCallback));
            collection.Add(new MyData(10));
            collection.Add(new MyData(20));
            collection.Add(new MyData(30));
            collection.Add(new MyData(40));
            collection.Add(new MyData(50));
            VerifyPositions(collection, div);
            collection.Move(1, 2);
            VerifyPositions(collection, div);
            collection.RemoveAt(3);
            VerifyPositions(collection, div);
            collection[2] = new MyData(77);
            VerifyPositions(collection, div);
            collection.Clear();
            VerifyPositions(collection, div);
        }

        private static void VerifyPositions(IReadOnlyList<MyData> collection, Element div)
        {
            Assert.Equal(collection.Count, div.ChildCount);
            for (var index = 0; index < collection.Count; index++)
            {
                var data = collection[index];
                VerifyPosition(div, index, data.Counter.ToString());
            }
        }

        private static void VerifyPosition(Element div, int position, string value)
        {
            var child = (Element)div.GetChildAt(position);
            var current = child.GetAttribute("data-counter");
            Assert.Equal(value, current);
        }

        [Fact]
        [Obsolete("old method")]
        public void BindFlagAttributeBinds()
        {
            var div = Element.Create("div");
            var data = new MyData();
            div.BindFlagAttribute(new BindFlagAttributeOptions<MyData>
            {
                Attribute = "data-even",
                BindObject = data,
                Property = x => x.IsEven
            });
            Assert.True(div.HasAttribute("data-even"));
            data.Counter++;
            Assert.False(div.HasAttribute("data-even"));
        }

        [Fact]
        [Obsolete("old methods")]
        public void BindToggleClassBinds()
        {
            var div = Element.Create("div");
            var data = new MyData();
            div.BindToggleClass(new BindToggleClassOptions<MyData>
            {
                ClassName = "lala",
                BindObject = data,
                Property = x => x.IsEven
            });
            Assert.True(div.HasClass("lala"));
            data.Counter++;
            Assert.False(div.HasClass("lala"));
        }

        [Fact]
        [Obsolete("old method")]
        public void LaraFlagBinding()
        {
            var div = Element.Create("div");
            var builder = new LaraBuilder(div);
            var data = new MyData();
            builder.BindFlagAttribute("data-even1", data, () => data.IsEven);
            builder.BindFlagAttribute("data-even2", data, x => x.IsEven);
            Assert.True(div.HasAttribute("data-even1"));
            Assert.True(div.HasAttribute("data-even2"));
        }

        [Fact]
        public void BindableBaseHoldsEvents()
        {
            var counter = 0;
            var data = new MyData();
            data.PropertyChanged += (_, _) => counter++;
            data.BeginUpdate();
            data.Counter = 5;
            Assert.Equal(0, counter);
            data.EndUpdate();
            Assert.Equal(1, counter);
        }

        [Fact]
        [Obsolete("old methods")]
        public void InputBindingGetter()
        {
            var data = new MyInputData
            {
                MyValue = "hello"
            };
            var input = new HtmlInputElement();
            input.BindInput(new BindInputOptions<MyInputData>
            {
                Attribute = "value",
                BindObject = data,
                Property = x => x.MyValue
            });
            Assert.Equal("hello", input.Value);
            data.MyValue = "bye";
            Assert.Equal("bye", input.Value);
        }

        [Fact]
        [Obsolete("old methods")]
        public void InputBindingGetterLara()
        {
            var data = new MyInputData
            {
                MyValue = "hello"
            };
            var input = new HtmlInputElement();
            var builder = new LaraBuilder(input);
            builder.BindInput("value", data, x => x.MyValue);
            Assert.Equal("hello", input.Value);
            data.MyValue = "bye";
            Assert.Equal("bye", input.Value);
        }

        [Fact]
        [Obsolete("old methods")]
        public void InputBindingGetterLaraFlag()
        {
            var data = new MyInputData
            {
                MyChecked = true
            };
            var input = new HtmlInputElement();
            var builder = new LaraBuilder(input);
            builder.BindFlagInput("checked", data, x => x.MyChecked);
            Assert.True(input.Checked);
            data.MyChecked = false;
            Assert.False(input.Checked);
        }

        [Fact]
        public void InputBindingSetter()
        {
            var data = new MyInputData
            {
                MyValue = "hello"
            };
            var input = new HtmlInputElement();
            var binding = new BindInputOptions<MyInputData>
            {
                Attribute = "value",
                BindObject = data,
                Property = x => x.MyValue
            };
            binding.Verify();
            input.Value = "bye";
            binding.Collect(input);
            Assert.Equal("bye", data.MyValue);
        }

        [Fact]
        public void InputBindingFlagSetter()
        {
            var data = new MyInputData
            {
                MyChecked = true
            };
            var input = new HtmlInputElement();
            var binding = new BindFlagInputOptions<MyInputData>
            {
                Attribute = "checked",
                BindObject = data,
                Property = x => x.MyChecked
            };
            binding.Verify();
            input.Checked = false;
            binding.Collect(input);
            Assert.False(data.MyChecked);
        }

        [Fact]
        [Obsolete("old methods")]
        public void InvalidSetterThrows()
        {
            var data = new MyInputData();
            var x = new HtmlInputElement();
            Assert.ThrowsAny<ArgumentException>(() =>
            {
                x.BindInput(new BindInputOptions<MyInputData>
                {
                    Attribute = "value",
                    BindObject = data,
                    Property = x1 => x1.MyValue + "a"
                });
            });
        }

        [Fact]
        [Obsolete("old methods")]
        public void InputBindingCollects()
        {
            var input = new HtmlInputElement();
            var data = new MyInputData();
            input.BindInput(new BindInputOptions<MyInputData>
            {
                BindObject = data,
                Attribute = "value",
                Property = x => x.MyValue
            });
            input.Value = "hello";
            Assert.Equal("hello", data.MyValue);
        }

        [Fact]
        [Obsolete("old methods")]
        public void InputBindingCollectsFlag()
        {
            var input = new HtmlInputElement();
            var data = new MyInputData();
            input.BindFlagInput(new BindFlagInputOptions<MyInputData>
            {
                BindObject = data,
                Attribute = "checked",
                Property = x => x.MyChecked
            });
            input.Checked = true;
            Assert.True(data.MyChecked);
        }

        [Fact]
        public void MissingMemberText()
        {
            var text = BindOptions.MissingMemberText("lala");
            Assert.Equal("Missing binding options member: lala", text);
        }

        [Fact]
        public void BindInputMissingProperty()
        {
            var x = new BindInputOptions<MyInputData>()
            {
                BindObject = new MyInputData(),
                Attribute = "value"
            };
            Assert.ThrowsAny<InvalidOperationException>(() => x.Verify());
        }

        [Fact]
        public void BindHandlerOptionsVerify()
        {
            var x = new BindHandlerOptions<MyInputData>
            {
                BindObject = new MyInputData()
            };
            Assert.ThrowsAny<InvalidOperationException>(() => x.Verify());
        }

        [Fact]
        public void BindInnerTextOptionsMissingProperty()
        {
            var x = new BindInnerTextOptions<MyInputData>
            {
                BindObject = new MyInputData()
            };
            Assert.ThrowsAny<InvalidOperationException>(() => x.Verify());
        }

        [Fact]
        public void BindInnerTextMissingData()
        {
            var x = new BindInnerTextOptions<MyInputData>
            {
                Property = _ => "lala"
            };
            Assert.ThrowsAny<InvalidOperationException>(() => x.Verify());
        }

        [Fact]
        [Obsolete("old method")]
        public void LaraBindFlagAttribute()
        {
            var data = new MyInputData();
            var div = Element.Create("div");
            var builder = new LaraBuilder(div);
            builder.BindFlagAttribute(new BindFlagAttributeOptions<MyInputData>
            {
                Attribute = "class",
                BindObject = data,
                Property = x => x.MyChecked
            });
            data.MyChecked = true;
            Assert.True(div.HasAttribute("class"));
        }

    }
}
