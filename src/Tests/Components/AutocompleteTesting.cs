/*
Copyright (c) 2019 Integrative Software LLC
Created: 11/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Autocomplete;
using Integrative.Lara.Delta;
using Integrative.Lara.Tests.Main;
using Integrative.Lara.Tests.Middleware;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Integrative.Lara.Tests.Components
{
    class MyProvider : IAutocompleteProvider
    {
        public Task<AutocompleteResponse> GetAutocompleteList(string term)
        {
            var list = new List<AutocompleteEntry>();
            var response = new AutocompleteResponse
            {
                Suggestions = list
            };
            list.Add(new AutocompleteEntry
            {
                Code = "R",
                Label = "Red"
            });
            list.Add(new AutocompleteEntry
            {
                Code = "G",
                Label = "Green"
            });
            list.Add(new AutocompleteEntry
            {
                Code = "B",
                Label = "Blue"
            });
            return Task.FromResult(response);
        }
    }

    public class AutocompleteTesting : DummyContextTesting
    {
        public AutocompleteTesting()
        {
            _context.Application.PublishComponent(new WebComponentOptions
            {
                ComponentTagName = AutocompleteElement.CustomTag,
                ComponentType = typeof(AutocompleteElement)
            });
        }

        [Fact]
        public void InnerInputValue()
        {
            var x = new AutocompleteElement
            {
                Value = "abc"
            };
            Assert.Equal("abc", x.Value);
            Assert.Equal("abc", x.InnerInput.Value);
        }

        [Fact]
        public void AutocompleteOptionsStore()
        {
            var provider = new MyProvider();
            var options = new AutocompleteOptions
            {
                Provider = provider,
                AutoFocus = true,
                MinLength = 2,
                Strict = true
            };
            Assert.Same(provider, options.Provider);
            Assert.True(options.AutoFocus);
            Assert.Equal(2, options.MinLength);
            Assert.True(options.Strict);
        }

        [Fact]
        public void AutocompleteStarts()
        {
            LaraUI.InternalContext.Value = _context;
            var x = new AutocompleteElement();
            var provider = new MyProvider();
            var options = new AutocompleteOptions
            {
                Provider = provider,
                AutoFocus = true,
                MinLength = 2,
                Strict = true
            };
            x.Start(options);
            var doc = new Document(new MyPage(), 100);
            var bridge = new Mock<IJSBridge>();
            _context.JSBridge = bridge.Object;
            
            var code = "LaraUI.autocompleteApply(context.Payload);";
            var payload = new AutocompletePayload
            {
                AutoFocus = options.AutoFocus,
                ElementId = x.InnerInput.EnsureElementId(),
                MinLength = options.MinLength,
                Strict = options.Strict
            };
            var json = LaraUI.JSON.Stringify(payload);

            bridge.Setup(x => x.Submit(code, json));
            doc.Body.AppendChild(x);
            bridge.Verify(x => x.Submit(code, json), Times.Once);
        }

        [Fact]
        public void AutocompleteStartStop()
        {
            LaraUI.InternalContext.Value = _context;
            var x = new AutocompleteElement();
            var provider = new MyProvider();
            var options = new AutocompleteOptions
            {
                Provider = provider,
                AutoFocus = true,
                MinLength = 2,
                Strict = true
            };

            var doc = new Document(new MyPage(), 100);
            var bridge = new Mock<IJSBridge>();
            _context.JSBridge = bridge.Object;
            doc.Body.AppendChild(x);

            x.Start(options);
            Assert.Equal(1, AutocompleteService.RegisteredCount);

            x.Stop();
            Assert.Equal(0, AutocompleteService.RegisteredCount);
        }

        [Fact]
        public void OnDisconnectStops()
        {
            LaraUI.InternalContext.Value = _context;
            var x = new AutocompleteElement();
            var provider = new MyProvider();
            var options = new AutocompleteOptions
            {
                Provider = provider,
                AutoFocus = true,
                MinLength = 2,
                Strict = true
            };

            var doc = new Document(new MyPage(), 100);
            var bridge = new Mock<IJSBridge>();
            _context.JSBridge = bridge.Object;
            doc.Body.AppendChild(x);

            x.Start(options);
            Assert.Equal(1, AutocompleteService.RegisteredCount);
            Assert.Same(options, x.GetOptions());

            x.Remove();
            Assert.Equal(0, AutocompleteService.RegisteredCount);
        }

        [Fact]
        public void AutocompleteEntry()
        {
            var x = new AutocompleteEntry
            {
                Code = "a",
                Html = "b",
                Label = "c",
                Subtitle = "d"
            };
            Assert.Equal("a", x.Code);
            Assert.Equal("b", x.Html);
            Assert.Equal("c", x.Label);
            Assert.Equal("d", x.Subtitle);
        }

        [Fact]
        public void AutocompleteResponse()
        {
            var list = new List<AutocompleteEntry>();
            var x = new AutocompleteResponse
            {
                Suggestions = list
            };
            Assert.Same(list, x.Suggestions);
        }

        [Fact]
        public async void AutocompleteServiceRun()
        {
            LaraUI.InternalContext.Value = _context;
            var x = new AutocompleteElement();
            var provider = new MyProvider();
            var options = new AutocompleteOptions
            {
                Provider = provider,
                AutoFocus = true,
                MinLength = 2,
                Strict = true,                
            };
            var doc = new Document(new MyPage(), 100);
            var bridge = new Mock<IJSBridge>();
            _context.JSBridge = bridge.Object;
            doc.Body.AppendChild(x);
            x.Start(options);

            var service = new AutocompleteService();
            var request = new AutocompleteRequest
            {
                Key = x.AutocompleteId,
                Term = "B"
            };
            _context.RequestBody = LaraUI.JSON.Stringify(request);
            var text = await service.Execute();
            var response = LaraUI.JSON.Parse<AutocompleteResponse>(text);
            Assert.Equal(3, response.Suggestions.Count);
            var item = response.Suggestions[0];
            Assert.Equal("Red", item.Label);
            Assert.Equal("R", item.Code);
        }

        [Fact]
        public void RegistryReplacesEntries()
        {
            LaraUI.InternalContext.Value = _context;
            var x = new AutocompleteRegistry();
            var auto1 = new AutocompleteElement();
            var auto2 = new AutocompleteElement();
            x.Set("a", auto1);
            x.Set("a", auto2);
            Assert.True(x.TryGet("a", out var autoX));
            Assert.Same(auto2, autoX);
        }

        [Fact]
        public async void ExecuteNotFoundReturnsEmpty()
        {
            var x = new AutocompleteService();
            var request = new AutocompleteRequest
            {
                Key = "a",
                Term = ""
            };
            var json = LaraUI.JSON.Stringify(request);
            var result = await x.Execute(json);
            Assert.Equal(string.Empty, result);
        }

    }
}
