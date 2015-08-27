#region License

// Copyright (c) 2015 FCDM
// Permission is hereby granted, free of charge, to any person obtaining 
// a copy of this software and associated documentation files (the "Software"), 
// to deal in the Software without restriction, including without limitation the 
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is furnished 
// to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all 
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

#region Header

/* Author: Michael Ala
 * 
 * Description
 * ===========
 * Extensions for the standard library XmlElement class.
 */

#endregion

#region Using Statements

using System.Collections.Generic;
using System.Linq;
using System.Xml;

#endregion

namespace DemeterEngine.Extensions
{
	public static class XmlElementExtensions
	{

        /// <summary>
        /// Return only the child elements of an xml element.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static List<XmlElement> ChildElements(this XmlElement element)
        {
            return element.ChildNodes.OfType<XmlElement>().ToList();
        }

		/// <summary>
		/// Find a child with a given name in an element.
		/// </summary>
		/// <param name="element"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static XmlElement FindChild(this XmlElement element, string name)
		{
			foreach (var child in element.ChildElements())
				if (child.Name == name)
					return child;
			return null;
		}

		/// <summary>
		/// Determine if a node has no children.
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		public static bool IsTerminal(this XmlElement element)
		{
			return !element.HasChildNodes ||
				   element.ChildNodes
						  .Cast<XmlNode>()
						  .All(item => item is XmlText || item is XmlComment);
		}

		/// <summary>
		/// Find a terminal child with a given name in an element.
		/// </summary>
		/// <param name="element"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static XmlElement FindTerminalChild(this XmlElement element, string name)
		{
			foreach (var child in element.ChildElements())
			{
				if (child.Name == name && child.IsTerminal())
					return child;
			}
			return null;
		}

	}
}
