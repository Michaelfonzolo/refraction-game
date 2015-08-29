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

using DemeterEngine.Extensions;

using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

#endregion

namespace Refraction_V2
{
	public static class OptionsManager
    {

        #region Xml Name Constants

        private const string OPTIONS_ELEMENT = "Options";

        private const string VOLUMES_ELEMENT = "Volumes";

        private const string KEYBINDINGS_ELEMENT = "KeyBindings";

        private const string SFX_VOL_ELEMENT = "SoundEffects";

        private const string MUSIC_VOL_ELEMENT = "Music";

        private const string BINDING_ELEMENT = "Binding";

        private const string KEY_NAME_ATTRIBUTE = "Key";

        private const string BINDING_NAME_ATTRIBUTE = "Name";

        #endregion

        internal static void Load(XmlDocument doc)
		{
			if (doc == null)
				return;

			var roots          = doc.ChildNodes;
			var configElement  = roots[1] as XmlElement;
			var optionsElement = configElement.FindChild(OPTIONS_ELEMENT);

			foreach (var child in optionsElement.ChildNodes.Cast<XmlElement>())
            {
                switch (child.Name)
                {
                    case VOLUMES_ELEMENT:
                        LoadVolumes(child);
                        break;
                    case KEYBINDINGS_ELEMENT:
                        LoadKeyBindings(child);
                        break;
                }
            }
		}

        internal static void LoadVolumes(XmlElement volumes)
        {
            var sfxVolumeElement   = volumes.FindTerminalChild(SFX_VOL_ELEMENT);
            var musicVolumeElement = volumes.FindTerminalChild(MUSIC_VOL_ELEMENT);

            Volumes.SoundEffects   = Convert.ToDouble(sfxVolumeElement.InnerText);
            Volumes.Music          = Convert.ToDouble(musicVolumeElement.InnerText);
        }

        internal static void LoadKeyBindings(XmlElement keybindings)
        {
            foreach (var child in keybindings.ChildNodes.Cast<XmlElement>())
            {
                Keys key;
                Enum.TryParse<Keys>(child.Attributes[KEY_NAME_ATTRIBUTE].InnerText, out key);
                KeyBindings.Bindings[child.Attributes[BINDING_NAME_ATTRIBUTE].InnerText] = key;
            }
        }

		internal static void Close(XmlWriter writer)
		{
			writer.WriteStartElement(OPTIONS_ELEMENT);

            WriteVolumes(writer);
            WriteKeyBindings(writer);

			writer.WriteEndElement();
		}

        internal static void WriteVolumes(XmlWriter writer)
        {
            writer.WriteStartElement(VOLUMES_ELEMENT);

            writer.WriteElementString(SFX_VOL_ELEMENT, Volumes.SoundEffects.ToString());
            writer.WriteElementString(MUSIC_VOL_ELEMENT, Volumes.Music.ToString());

            writer.WriteEndElement();
        }

        internal static void WriteKeyBindings(XmlWriter writer)
        {
            writer.WriteStartElement(KEYBINDINGS_ELEMENT);

            foreach (var keybinding in KeyBindings.Bindings)
            {
                writer.WriteStartElement(BINDING_ELEMENT);
                writer.WriteAttributeString(BINDING_NAME_ATTRIBUTE, keybinding.Key);
                writer.WriteAttributeString(KEY_NAME_ATTRIBUTE, keybinding.Value.ToString());
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        public static class Volumes
        {

            public static double SoundEffects = 1d;

            public static double Music = 1d;

        }

        public static class KeyBindings
        {

            public static readonly string KeyName_Escape = "Escape";

            public static readonly Keys DefaultKey_Escape = Keys.Escape;

            private static readonly Dictionary<string, Keys> DefaultBindings = new Dictionary<string, Keys>()
            {
                {KeyName_Escape, Keys.Escape}
            };

            public static readonly Dictionary<string, Keys> Bindings;

            static KeyBindings()
            {
                Bindings = new Dictionary<string, Keys>();
                foreach (var kvp in DefaultBindings)
                {
                    Bindings.Add(kvp.Key, kvp.Value);
                }
            }

            /// <summary>
            /// Set a keybinding. The return value of this function is a bool indicating whether
            /// or not the new key is valid. A key is invalid if it is already used 
            /// </summary>
            /// <param name="keyName"></param>
            /// <param name="key"></param>
            /// <returns></returns>
            public static bool SetBinding(string keyName, Keys key)
            {
                foreach (var binding in Bindings)
                {
                    if (binding.Value == key)
                        return false;
                }
                Bindings[keyName] = key;
                return true;
            }

            /// <summary>
            /// Reset the keybindings to their default values.
            /// </summary>
            public static void ResetBindingsToDefault() 
            {
                foreach (var kvp in DefaultBindings)
                {
                    Bindings[kvp.Key] = kvp.Value;
                }
            }

        }

	}
}
