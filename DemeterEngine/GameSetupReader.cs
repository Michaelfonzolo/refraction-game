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
 * The GameSetupReader reads a .gamesetup xml file and initialize's the game's
 * basic global properties. The .gamesetup file controls the following properties:
 * 
 * - The resolution of the window
 * - The name of the folder from which to load content
 * - The default colour to fill the background with before every new render cycle.
 * - Whether or not the game is fullscreen on startup
 * - Whether or not the game can be switched to fullscreen during gameplay
 * - Whether or not the game is borderless on startup
 * - Whether or not the game can switch between bordered and borderless during gameplay
 * - Whether or not the mouse is visible during gameplay
 * - Whether or not the mouse visibility is togglable during gameplay.
 */

#endregion

#region Using Statements

using DemeterEngine.Extensions;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;

#endregion

namespace DemeterEngine
{
    public class GameSetupReader
    {

        /// <summary>
        /// The names of the various elements that are expected.
        /// </summary>
        #region Element Names

        public const string ROOT_ELEMENT                          = "GameSetup";
        public const string CONTENT_FOLDER_ELEMENT                = "ContentFolder";
        public const string WINDOW_RESOLUTION_ELEMENT             = "WindowResolution";
        public const string FULLSCREEN_ON_STARTUP_ELEMENT         = "Fullscreen";
        public const string ALLOW_FULLSCREEN_TOGGLE_ELEMENT       = "AllowFullscreenToggle";
        public const string BORDERLESS_ON_STARTUP_ELEMENT         = "Borderless";
        public const string ALLOW_BORDERLESS_TOGGLE_ELEMENT       = "AllowBorderlessnessToggle";
        public const string MOUSE_VISIBLE_ELEMENT                 = "MouseVisible";
        public const string ALLOW_MOUSE_VISIBILITY_TOGGLE_ELEMENT = "AllowMouseVisibilityToggle";        
        public const string BACKGROUND_FILL_COLOUR_ELEMENT        = "BackgroundFillColour";
        public const string VSYNC_ELEMENT                         = "VSync";

        #endregion

        /// <summary>
        /// The indicator that can be used by the WindowResolution element to 
        /// indicate to use the system's native resolution as the window resolution.
        /// </summary>
        private const string INDICATOR_NATIVE_RESOLUTION = "native";

        /// <summary>
        /// The Regex pattern for a resolution ([int]x[int], i.e. 800x600).
        /// </summary>
        private const string RESOLUTION_REGEX = @"[0-9]+x[0-9]+$";

        /// <summary>
        /// The Regex pattern for a colour (a 6-digit hex number, i.e. 0x15d3cf).
        /// </summary>
        private const string COLOR_REGEX = @"0(x|X)[0-9a-fA-F]{6}$";

        /// <summary>
        /// The default background fill colour.
        /// </summary>
        private static readonly Color DEFAULT_BGFILL_COL = Color.Black;

        /// <summary>
        /// The folder from which to load content via the content pipeline.
        /// </summary>
        public string ContentFolder { get; private set; }

        /// <summary>
        /// The resolution of the game window.
        /// </summary>
        public Resolution WindowResolution { get; private set; }

        /// <summary>
        /// The fill colour of the background.
        /// </summary>
        public Color BackgroundFillColour { get; private set; }

        /// <summary>
        /// Whether or not the window is fullscreen by default.
        /// </summary>
        public bool FullscreenOnStartup { get; private set; }

        /// <summary>
        /// Whether or not fullscreen toggling is enabled.
        /// </summary>
        public bool FullscreenTogglable { get; private set; }

        /// <summary>
        /// Whether or not the window is borderless by default.
        /// </summary>
        public bool BorderlessOnStartup { get; private set; }

        /// <summary>
        /// Whether or not borderlessness toggling is enabled.
        /// </summary>
        public bool BorderlessnessTogglable { get; private set; }

        /// <summary>
        /// Whether or not the mouse is visible.
        /// </summary>
        public bool MouseVisible { get; private set; }

        /// <summary>
        /// Whether or not mouse visibility toggling is enabled.
        /// </summary>
        public bool MouseVisibilityTogglable { get; private set; }

        /// <summary>
        /// Whether or not to use vertically aligned retrace.
        /// </summary>
        public bool VSync { get; private set; }

        /// <summary>
        /// Whether or not we have already read the gamesetup file.
        /// </summary>
        private bool done = false;

		public string FileName { get; private set; }

        public GameSetupReader(string fileName)
        {
			FileName = fileName;
        }

		private void Error_UnexpectedRootNodes()
		{
			throw new GameSetupFileException("Expected an XmlDeclaration and only one root element.");
		}

		private void Error_UnexpectedRootNodeName(string name)
		{
			throw new GameSetupFileException(
				String.Format("Expected root element with name 'GameSetup', got '{0}' instead.", name)
				);
		}

		private void Error_NoTerminalChildFound(string name, string parentName)
		{
			throw new GameSetupFileException(
				String.Format(
					"No terminal child element with name '{0}' found in '{1}' element.", name, parentName
					)
				);
		}

		private void Error_InvalidResolutionFormat(string resolution)
		{
			throw new GameSetupFileException(
				String.Format(
					"Invalid format for resolution '{0}'. Must be of the form '[int]x[int]'.", resolution
					)
				);
		}

		private void Debug_CouldNotParseBool(string elementName, string innerText, bool defaultVal)
		{
			Debug.WriteLine(
				String.Format(
					"(GameSetup) - Could not parse {0}'s text '{1}' as bool, setting to default value '{2}'...",
					elementName, innerText, defaultVal
					)
				);
		}

		private void Debug_CouldNotParseColour(string colour)
		{
			Debug.WriteLine(
				String.Format(
					"(GameSetup) - Could not parse {0}'s text '{1}' as Color, setting to default value '{2}'...",
					BACKGROUND_FILL_COLOUR_ELEMENT, colour, DEFAULT_BGFILL_COL
					)
				);
		}

        /// <summary>
        /// Read the entire .gamesetup file. The returned bool indicates whether the file
        /// has already been read or not.
        /// </summary>
        /// <returns></returns>
        public bool ReadAll()
        {
            if (done)
                return false;

			var doc = new XmlDocument();
			doc.Load(FileName);

            var roots = doc.ChildNodes;
            if (roots.Count != 2)
                Error_UnexpectedRootNodes();

			var root = roots[1] as XmlElement;
            if (root.Name != ROOT_ELEMENT)
                Error_UnexpectedRootNodeName(root.Name);

            var contentFolderElement = AttemptFind(root, CONTENT_FOLDER_ELEMENT);
            ContentFolder = contentFolderElement.InnerText;

            var resolutionElement = AttemptFind(root, WINDOW_RESOLUTION_ELEMENT);
            WindowResolution = ReadResolution(resolutionElement.InnerText);

            var colourElement = root.FindChild(BACKGROUND_FILL_COLOUR_ELEMENT);
            if (colourElement == null)
                BackgroundFillColour = DEFAULT_BGFILL_COL;
            else
                BackgroundFillColour = ReadColour(colourElement.InnerText);

            FullscreenOnStartup      = ReadBool(root, FULLSCREEN_ON_STARTUP_ELEMENT);
            FullscreenTogglable      = ReadBool(root, ALLOW_FULLSCREEN_TOGGLE_ELEMENT);

            BorderlessOnStartup      = ReadBool(root, BORDERLESS_ON_STARTUP_ELEMENT);
            BorderlessnessTogglable  = ReadBool(root, ALLOW_BORDERLESS_TOGGLE_ELEMENT);

            MouseVisible             = ReadBool(root, MOUSE_VISIBLE_ELEMENT);
            MouseVisibilityTogglable = ReadBool(root, ALLOW_MOUSE_VISIBILITY_TOGGLE_ELEMENT);

            VSync = ReadBool(root, VSYNC_ELEMENT);

            done = true;
            return done;
        }

        /// <summary>
        /// Attempt to find a terminal child of the given root with the given name, and
        /// throw an appropriate GameSetupFileException if not found.
        /// </summary>
        private XmlElement AttemptFind(XmlElement root, string name)
        {
            var element = root.FindTerminalChild(name);
			if (element == null)
				Error_NoTerminalChildFound(name, root.Name);
            return element;
        }

        /// <summary>
        /// Read an element whose text is to be interpretted as a bool and
        /// return the result.
        /// </summary>
        private bool ReadBool(XmlElement root, string elementName, bool defaultVal = false)
        {
            var element = root.FindChild(elementName);
            if (element == null)
                return defaultVal;
            else
            {
                try
                {
                    return Convert.ToBoolean(element.InnerText.ToLower());
                }
                catch (FormatException)
                {
					Debug_CouldNotParseBool(elementName, element.InnerText, defaultVal);
                    return defaultVal;
                }
            }
        }

        /// <summary>
        /// Parse a string as a Resolution object and return the result.
        /// </summary>
        private Resolution ReadResolution(string resolution)
        {
            if (resolution == INDICATOR_NATIVE_RESOLUTION)
                return Resolution.Native;
			if (!Regex.IsMatch(resolution, RESOLUTION_REGEX))
				Error_InvalidResolutionFormat(resolution);

            var parts = resolution.Split('x');

            var width = Int32.Parse(parts[0]);
            var height = Int32.Parse(parts[1]);
            return new Resolution(width, height);
        }

        /// <summary>
        /// Parse a string as a Color object and return the result.
        /// </summary>
        private Color ReadColour(string colour)
        {
            // If the given colour string is a name, attempt to find it as a static property
            // of Color. For example, any case-permutation of 'black' will correspond to
            // Color.Black.
            var prop = typeof(Color).GetProperty(

                    // So apparently this is the only builtin way to convert to title case...
                    new CultureInfo("en-US")
                        .TextInfo
                        .ToTitleCase(colour),

                    BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy
                    );
            if (prop != null)
                return (Color)prop.GetValue(null, null);
            if (!Regex.IsMatch(colour, COLOR_REGEX))
            {
				Debug_CouldNotParseColour(colour);
                return DEFAULT_BGFILL_COL;
            }
            var val = Convert.ToInt32(colour, 16);
            var r = 0xff & (val >> 16);
            var g = 0xff & (val >> 8);
            var b = 0xff & val;
            return new Color(r, g, b);
        }
    }
}
