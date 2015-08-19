using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using DemeterEngine.Extensions;

namespace Refraction_V2
{
	public static class OptionsManager
	{

		private const string OPTIONS_ELEMENT = "Options";

		internal static void Load(XmlDocument doc)
		{
			if (doc == null)
				return;

			var roots          = doc.ChildNodes;
			var configElement  = roots[1] as XmlElement;
			var optionsElement = configElement.FindChild(OPTIONS_ELEMENT);

			// Do stuff
		}

		internal static void Close(XmlWriter writer)
		{
			writer.WriteStartElement(OPTIONS_ELEMENT);
			writer.WriteEndElement();
		}

	}
}
