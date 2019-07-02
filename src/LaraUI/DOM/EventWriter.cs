/*
Copyright (c) 2019 Integrative Software LLC
Created: 6/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Delta;
using System;
using System.Threading.Tasks;
using System.Web;

namespace Integrative.Lara.DOM
{
    sealed class EventWriter
    {
        readonly Element _element;
        readonly EventSettings _settings;

        public static void On(Element element, string eventName, Func<IPageContext, Task> handler)
        {
            On(element, new EventSettings
            {
                EventName = eventName,
                Handler = handler
            });
        }

        public static void On(Element element, EventSettings settings)
        {
            new EventWriter(element, settings).Write();
        }

        private EventWriter(Element element, EventSettings settings)
        {
            _element = element;
            _settings = settings;
        }

        private void Write()
        {
            string eventName = HttpUtility.HtmlEncode(_settings.EventName.ToLowerInvariant());
            _element.Events.Remove(eventName);
            if (_settings.Handler == null)
            {
                RemoveEvent(eventName);
            }
            else
            {
                _element.Events.Add(eventName, _settings.Handler);
                WriteEvent(eventName);
            }
        }

        private void RemoveEvent(string eventName)
        {
            var attribute = GetEventAttribute(eventName);
            _element.RemoveAttribute(attribute);
            attribute = GetDataAttribute(eventName);
            _element.RemoveAttribute(attribute);
        }

        private void WriteEvent(string eventName)
        {
            WriteEventAttribute(eventName);
            WriteDataAttribute(eventName);
        }

        private void WriteEventAttribute(string eventName)
        {
            var eventAttribute = GetEventAttribute(eventName);
            var code = BuildEventCode();
            _element.SetAttribute(eventAttribute, code);
        }

        private void WriteDataAttribute(string eventName)
        {
            var eventData = GetDataAttribute(eventName);
            var data = BuildEventData();
            _element.SetAttribute(eventData, data);
        }

        private string BuildEventCode()
        {
            return $"LaraUI.plug(this, '{_settings.EventName}');";
        }

        private string BuildEventData()
        {
            var plug = new PlugOptions(_settings)
            {
                EventName = null
            };
            return plug.ToJSON();
        }

        private static string GetEventAttribute(string eventName)
        {
            return "on" + eventName;
        }

        private static string GetDataAttribute(string eventName)
        {
            return "data-lara-event-" + eventName;
        }
    }
}
