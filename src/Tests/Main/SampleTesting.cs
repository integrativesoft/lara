/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.DOM;
using Integrative.Lara.Main;
using Integrative.Lara.Middleware;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Integrative.Lara.Tests.Main
{
    public class SampleTesting : IDisposable
    {
        readonly IWebDriver _driver;

        public SampleTesting()
        {
            _driver = new FirefoxDriver(Environment.CurrentDirectory);
            PostEventHandler.EventComplete += PostEventHandler_EventComplete;
            LaraUI.ClearAll();
        }

        private int _counter = 0;

        private void PostEventHandler_EventComplete(object sender, EventArgs e)
        {
            _counter++;
        }

        public void Dispose()
        {
            _driver.Close();
        }

        [Fact]
        public async void CounterButton()
        {
            var page = new ButtonCounterPage();
            LaraUI.Publish("/", () => page);
            using (var host = await LaraUI.StartServer())
            {
                string address = LaraUI.GetFirstURL(host);
                _driver.Navigate().GoToUrl(address);
                var button = _driver.FindElement(By.Id(ButtonCounterPage.ButtonId));
                string before = button.Text;

                await WaitForEvent(() => button.Click());
                string after1 = button.Text;
                string path = page.LastPath;
                
                await WaitForEvent(() => button.Click());
                string after2 = button.Text;

                Assert.Equal("/_event", path);
                Assert.Equal("Click me", before);
                Assert.Equal("Clicked 1 times", after1);
                Assert.Equal("Clicked 2 times", after2);
            }
        }

        [Fact]
        public async void SecondPageReusesConnection()
        {
            LaraUI.Publish("/", () => new ButtonCounterPage());
            using (var host = await LaraUI.StartServer())
            {
                string address = LaraUI.GetFirstURL(host);
                _driver.Navigate().GoToUrl(address);

                var published = LaraUI.GetPublished();
                var pair1 = published.Connections.GetConnections().FirstOrDefault();

                _driver.Navigate().GoToUrl(address);
                int count = published.Connections.GetConnections().Count();
                var pair2 = published.Connections.GetConnections().FirstOrDefault();

                Assert.Equal(1, count);
                Assert.Equal(pair1.Key, pair2.Key);
                Assert.Same(pair1.Value, pair2.Value);
            }
        }

        [Fact]
        public async void FocusFocuses()
        {
            LaraUI.Publish("/", () => new TwoButtonPage());
            using (var host = await LaraUI.StartServer())
            {
                string address = LaraUI.GetFirstURL(host);
                _driver.Navigate().GoToUrl(address);
                var b1 = _driver.FindElement(By.Id("b1"));
                await WaitForEvent(() => b1.Click());

                var b3 = _driver.FindElement(By.Id("b3"));
                var sel = _driver.SwitchTo().ActiveElement();
                Assert.True(b3.Equals(sel));
            }
        }

        class TwoButtonPage : IPage
        {
            public Task OnGet(IPageContext context)
            {
                var b1 = Element.Create("button", "b1");
                b1.AppendChild(new TextNode("one"));
                var b2 = Element.Create("button", "b2");
                b2.AppendChild(new TextNode("two"));
                var b3 = Element.Create("button", "b3");
                b3.AppendChild(new TextNode("three"));
                context.Document.Body.AppendChild(b1);
                context.Document.Body.AppendChild(b2);
                context.Document.Body.AppendChild(b3);
                b1.On("click", app =>
                {
                    b3.Focus();
                    return Task.CompletedTask;
                });
                return Task.CompletedTask;
            }
        }

        private async Task WaitForEvent(Action action)
        {
            _counter = 0;
            action();
            while (_counter == 0)
            {
                await Task.Delay(50);
            }
        }

        [Fact]
        public async void EnterInputValue()
        {
            var page = new MySubmitPage();
            LaraUI.Publish("/", () => page);
            using (var host = await LaraUI.StartServer())
            {
                string address = LaraUI.GetFirstURL(host);
                _driver.Navigate().GoToUrl(address);

                var input = _driver.FindElement(By.TagName("input"));
                input.SendKeys("hello!");

                var b1 = _driver.FindElement(By.TagName("button"));
                await WaitForEvent(() => b1.Click());

                Assert.Equal("hello!", page.Value);
            }
        }

        class MySubmitPage : IPage
        {
            public string Value { get; private set; }

            public Task OnGet(IPageContext context)
            {
                var submit = Element.Create("button");
                submit.AppendChild(new TextNode("submit"));
                var input = Element.Create("input");
                context.Document.Body.AppendChild(input);
                context.Document.Body.AppendChild(submit);
                submit.On("click", app =>
                {
                    Value = input.GetAttribute("value");
                    return Task.CompletedTask;
                });
                return Task.CompletedTask;
            }
        }

        [Fact]
        public async void ExecuteJavascript()
        {
            LaraUI.Publish("/", () => new MyJS());
            using (var host = await LaraUI.StartServer())
            {
                string address = LaraUI.GetFirstURL(host);
                _driver.Navigate().GoToUrl(address);
                var button = _driver.FindElement(By.TagName("button"));
                await WaitForEvent(() => button.Click());

                string text = button.Text;
                Assert.Equal("clicked!", text);
            }
        }

        class MyJS : IPage
        {
            const string Code = "document.getElementById('mybutton').innerText = 'clicked!';";

            public Task OnGet(IPageContext context)
            {
                var button = Element.Create("button", "mybutton");
                var text = new TextNode("test JS");
                button.AppendChild(text);
                button.On("click", app =>
                {
                    app.JSBridge.Submit(Code);
                    return Task.CompletedTask;
                });
                context.Document.Body.AppendChild(button);
                return Task.CompletedTask;
            }
        }

        class MyMessagePage : IPage
        {
            public string Received { get; private set; }

            public Task OnGet(IPageContext context)
            {
                var button = Element.Create("button", "mybutton");
                var text = new TextNode("test message");
                button.AppendChild(text);
                button.SetAttribute("onclick", "LaraUI.sendMessage( { key: 'hello', data: 'mydata' } );");
                context.Document.Body.AppendChild(button);
                context.JSBridge.OnMessage("hello", app =>
                {
                    Received = app.JSBridge.EventData;
                    return Task.CompletedTask;
                });
                return Task.CompletedTask;
            }
        }

        [Fact]
        public async void RunJsMessage()
        {
            var page = new MyMessagePage();
            LaraUI.Publish("/", () => page);
            using (var host = await LaraUI.StartServer())
            {
                string address = LaraUI.GetFirstURL(host);
                _driver.Navigate().GoToUrl(address);
                var button = _driver.FindElement(By.TagName("button"));
                await WaitForEvent(() => button.Click());

                Assert.Equal("mydata", page.Received);
            }
        }

        [Fact]
        public async void ReplacePost()
        {
            var page = new MyReplacePage();
            var redirect = new MyLandingPage();
            LaraUI.Publish("/", () => page);
            LaraUI.Publish("/landing", () => redirect);
            using (var host = await LaraUI.StartServer())
            {
                string address = LaraUI.GetFirstURL(host);
                _driver.Navigate().GoToUrl(address);
                var button = _driver.FindElement(By.TagName("button"));
                await WaitForEvent(() => button.Click());

                Assert.Equal(1, redirect.Counter);
            }
        }

        class MyReplacePage : IPage
        {
            public Task OnGet(IPageContext context)
            {
                var button = Element.Create("button", "mybutton");
                var text = new TextNode("test replace post");
                button.AppendChild(text);
                button.On("click", app =>
                {
                    app.Navigation.Replace("landing");
                    return Task.CompletedTask;
                });
                context.Document.Body.AppendChild(button);
                return Task.CompletedTask;
            }
        }

        class MyLandingPage : IPage
        {
            public int Counter { get; private set; } = 0;

            public Task OnGet(IPageContext context)
            {
                Counter++;
                return Task.CompletedTask;
            }
        }
    }
}
