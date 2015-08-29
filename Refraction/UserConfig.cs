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
 */

#endregion

#region Using Statements

using System.IO;
using System.Xml;

#endregion

namespace Refraction_V2
{
	public static class UserConfig
	{

		public const string USER_INFO_FILE_NAME = "userinfo.config";

		public static void Load()
		{
			var doc = new XmlDocument();
			try
			{
				doc.Load(USER_INFO_FILE_NAME);
			}
			catch (FileNotFoundException)
			{
				// The document will only be null the first time the game is played, before
				// any userinfo.config xml file exists.
				doc = null;
			}

			LoadedLevelManager.Load(doc);
			OptionsManager.Load(doc);
		}

		public static void Close()
		{
			using (var writer = XmlWriter.Create(USER_INFO_FILE_NAME))
			{
				writer.WriteStartDocument();
				writer.WriteStartElement("Config");

				LoadedLevelManager.Close(writer);
				OptionsManager.Close(writer);

				writer.WriteEndElement();
			}
		}

	}
}
