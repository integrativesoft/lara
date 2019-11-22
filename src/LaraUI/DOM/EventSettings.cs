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
    /// Propagation options for client events
    /// </summary>
    public enum PropagationType
    {
        /// <summary>
        /// Uses the 'StopPropagation' option
        /// </summary>
        Default = 0,

        /// <summary>
        /// Prevents the event to bubble up to parent elements
        /// </summary>
        StopPropagation = 1,

        /// <summary>
        /// Prevents the event to bubble up to parent elements or any other handler
        /// </summary>
        StopImmediatePropagation = 2,

        /// <summary>
        /// Allows event propagation
        /// </summary>
        AllowAll = 3
    }

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
        /// Specifies an interval in millisenconds to postpone and hold an event while it gets triggered repeatedly.
        /// The resulting 'debounced' event will execute only after it has not been triggered for the interval specified.
        /// </summary>
        public int DebounceInterval { get; set; }

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

        /// <summary>
        /// When set to true, the client will send the files from input elements of type 'file'
        /// </summary>
        public bool UploadFiles { get; set; }

        /// <summary>
        /// Defines JavaScript code to execute in order to determine if the event should trigger.
        /// When this property is set, the event is trigger only if the expression evaluates to true.
        /// </summary>
        public string EvalFilter { get; set; }

        /// <summary>
        /// Specifies propagation options for client events
        /// </summary>
        public PropagationType Propagation { get; set; }

        internal void Verify()
        {
            if (string.IsNullOrEmpty(EventName))
            {
                throw new ArgumentException(Resources.EventNameNull);
            }
            else if (LongRunning && UploadFiles)
            {
                throw new InvalidOperationException(Resources.LongRunningNoFiles);
            }
        }
    }
}
