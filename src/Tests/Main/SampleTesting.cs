/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Clara.Main;
using Integrative.Clara.Middleware;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Integrative.Clara.Tests.Main
{
    public class SampleTesting : IDisposable
    {
        readonly IWebDriver _driver;
        readonly TaskCompletionSource<bool> _tcs;

        public SampleTesting()
        {
            _driver = new FirefoxDriver(Environment.CurrentDirectory);
            _tcs = new TaskCompletionSource<bool>();
            PostEventHandler.EventComplete += PostEventHandler_EventComplete;
            ClaraUI.ClearAll();
        }

        private void PostEventHandler_EventComplete(object sender, EventArgs e)
        {
            _tcs.SetResult(true);
        }

        public void Dispose()
        {
            _driver.Close();
        }

        [Fact]
        public async void CounterButton()
        {
            var page = new ButtonCounterPage();
            ClaraUI.Publish("/", () => page);
            using (var host = await ClaraUI.StartServer())
            {
                string address = ClaraUI.GetFirstURL(host);
                _driver.Navigate().GoToUrl(address);
                var button = _driver.FindElement(By.Id(ButtonCounterPage.ButtonId));
                string before = button.Text;

                button.Click();
                await WaitForEvent();
                string after1 = button.Text;
                string path = page.LastPath;

                button.Click();
                await WaitForEvent();
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
            ClaraUI.Publish("/", () => new ButtonCounterPage());
            using (var host = await ClaraUI.StartServer())
            {
                string address = ClaraUI.GetFirstURL(host);
                _driver.Navigate().GoToUrl(address);

                var published = ClaraUI.GetPublished();
                var pair1 = published.Connections.GetConnections().FirstOrDefault();

                _driver.Navigate().GoToUrl(address);
                int count = published.Connections.GetConnections().Count();
                var pair2 = published.Connections.GetConnections().FirstOrDefault();

                Assert.Equal(1, count);
                Assert.Equal(pair1.Key, pair2.Key);
                Assert.Same(pair1.Value, pair2.Value);
            }
        }

        private async Task WaitForEvent()
        {
            await _tcs.Task;
        }
    }
}
