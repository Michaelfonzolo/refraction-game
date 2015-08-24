using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

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
            Console.WriteLine("Yo!");
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
