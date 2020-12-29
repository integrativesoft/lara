/*
Copyright (c) 2020 Integrative Software LLC
Created: 12/2020
Author: Pablo Carbonell
*/

using System;

namespace Integrative.Lara
{
    /// <summary>
    /// Represents the h1..h6 tags
    /// </summary>
    public class HtmlHeadingElement : Element
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="level"></param>
        public HtmlHeadingElement(int level) : base(GetLevelTag(level))
        {
        }

        /// <summary>
        /// Returns h1..h6 for levels 1..6
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static string GetLevelTag(int level)
        {
            if (level < 1 || level > 6)
            {
                throw new ArgumentException("Invalid heading level");
            }
            return $"h{level}";
        }
    }
}
