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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

#endregion

namespace Refraction_V2
{
	public static class LoadedLevelManager
    {

        #region Xml Element Name Constants

        private const string COMPLETED_LEVELS_ELEMENT = "CompletedLevels";

        private const string HIGHEST_UNLOCKED_LEVEL_ELEMENT = "HighestUnlockedLevel";

		private const string LEVEL_FOLDER = "Levels";

		private const string LEVEL_FOLDER_SEARCH_PATTERN = "*.levelfile";

        #endregion

        /// <summary>
		/// The sequential listing of levels.
		/// </summary>
		internal static string[] SequentialLevels { get; private set; }

		/// <summary>
		/// The list of levels the player has completed.
		/// </summary>
		internal readonly static HashSet<int> CompletedLevels = new HashSet<int>();

        /// <summary>
        /// The number of the highest unlocked level.
        /// </summary>
        internal static int HighestUnlockedLevel { get; private set; }

		private static int LevelToInt(string levelName)
		{
			return Convert.ToInt32(levelName.Split('.')[0]);
		}

		internal static void Load(XmlDocument doc)
		{
			var directory = new DirectoryInfo(LEVEL_FOLDER);
			var levelFiles = directory.GetFiles(LEVEL_FOLDER_SEARCH_PATTERN);

			Array.Sort(levelFiles, (x, y) => (LevelToInt(x.Name).CompareTo(LevelToInt(y.Name))));
			SequentialLevels = levelFiles.Select(f => f.FullName).ToArray();

            HighestUnlockedLevel = 0;

			if (doc == null)
				return;

			var roots                 = doc.ChildNodes;
			var configElement         = roots[1] as XmlElement;
			var completedLevelElement = configElement.FindChild(COMPLETED_LEVELS_ELEMENT);

			foreach (var num in completedLevelElement.InnerText.Trim().Split(' '))
			{
                // This only ever occurs when the user hasn't completed a single level,
                // which in turn cases the "CompletedLevels" element to get written as 
                // a terminal element (<CompletedLevels/>) with no actual inner text.
                if (num == "")
                    continue;
				CompletedLevels.Add(Convert.ToInt32(num.Trim()));
			}

            HighestUnlockedLevel = Convert.ToInt32(
                configElement.FindChild(HIGHEST_UNLOCKED_LEVEL_ELEMENT).InnerText);
		}

		internal static void Close(XmlWriter writer)
		{
			var completedLevelsText = new StringBuilder();

			int i = 0;
			foreach (var levelNum in LoadedLevelManager.CompletedLevels)
			{
				i++;
				completedLevelsText.Append(levelNum.ToString() + " ");
			}

			writer.WriteElementString(COMPLETED_LEVELS_ELEMENT, completedLevelsText.ToString());
            writer.WriteElementString(HIGHEST_UNLOCKED_LEVEL_ELEMENT, HighestUnlockedLevel.ToString());
		}
	}
}
