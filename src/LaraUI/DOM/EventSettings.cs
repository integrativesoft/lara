/*
Copyright (c) 2019 Integrative Software LLC
Created: 6/2019
Author: Pablo Carbonell
*/

using System;
using System.Threading.Tasks;

namespace Integrative.Lara
{
    /// <summary>
    /// Specifies settings to declare an event and associate execution code to it.
    /// </summary>
    public sealed class EventSettings
    {
        /// <summary>
        /// Name of the HTML5 event (e.g. 'click')
        /// </summary>
        /// <value>
        /// The name of the event.
        /// </value>
        public string EventName { get; set; }

        /// <summary>
        /// Block the UI while the event is executing?
        /// </summary>
        /// <value>
        ///   <c>true</c> if block; otherwise, <c>false</c>.
        /// </value>
        public bool Block { get; set; }

        /// <summary>
        /// Gets or sets the options for blocking the UI.
        /// </summary>
        /// <value>
        /// The block options.
        /// </value>
        public BlockOptions BlockOptions { get; set; }

        /// <summary>
        /// Gets or sets the code to execute for the event.
        /// </summary>
        /// <value>
        /// The handler.
        /// </value>
        public Func<Task> Handler { get; set; }

        /// <summary>
        /// Long-running events use websockets and can flush partial progress to the client.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [long running]; otherwise, <c>false</c>.
        /// </value>
        public bool LongRunning { get; set; }

        internal void Verify()
        {
            if (string.IsNullOrEmpty(EventName))
            {
                throw new ArgumentNullException(nameof(EventName));
            }
        }
    }
}
