/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 6/2019
Author: Pablo Carbonell
*/

namespace Integrative.Lara
{
    /// <summary>
    /// Defines options for blocking the UI while executing an event
    /// </summary>
    public class BlockOptions
    {
        /// <summary>
        /// Gets or sets the ID of element to block. Leave empty to block the whole page.
        /// </summary>
        /// <value>
        /// The ID of the element to block. If left blank, block the entire page.
        /// </value>
        public string? BlockedElementId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the element to show. If set, the element specified will be shown instead of the default block dialog.
        /// </summary>
        /// <value>
        /// The ID of the element to show instead of the default block dialog.
        /// </value>
        public string? ShowElementId { get; set; }

        /// <summary>
        /// Gets or sets an HTML message to show while blocking the user interface. If set, the specified HTML will be shown instead of the default block dialog.
        /// </summary>
        /// <value>
        /// The HTML message to show instead of the default block dialog.
        /// </value>
        public string? ShowHtmlMessage { get; set; }
    }
}
