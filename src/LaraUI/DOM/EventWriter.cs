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
                string attribute = GetEventAttribute(eventName);
                _element.RemoveAttribute(attribute);
            }
            else
            {
                _element.Events.Add(eventName, _settings.Handler);
                WriteEvent(eventName);
            }
        }

        private void WriteEvent(string eventName)
        {
            string attribute = GetEventAttribute(eventName);
            string value = BuildEventCode();
            _element.SetAttribute(attribute, value);
        }

        private string BuildEventCode()
        {
            var plug = new PlugOptions(_settings);
            var json = plug.ToJSON();
            return $"LaraUI.plug(this, {json});";
        }

        private static string GetEventAttribute(string eventName)
        {
            return "on" + eventName;
        }
    }
}
