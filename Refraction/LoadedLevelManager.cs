using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using DemeterEngine.Extensions;

namespace Refraction_V2
{
	public static class LoadedLevelManager
	{

		private const string COMPLETED_LEVELS_ELEMENT = "CompletedLevels";

		private const string LEVEL_FOLDER = "Levels";

		private const string LEVEL_FOLDER_SEARCH_PATTERN = "*.levelfile";

		/// <summary>
		/// The total number of levels in the sequential listing of levels.
		/// </summary>
		public static int NUM_SEQUENTIAL_LEVELS { get; private set; }

		/// <summary>
		/// The sequential listing of levels.
		/// </summary>
		internal static string[] SequentialLevels { get; private set; }

		/// <summary>
		/// The list of levels the player has completed.
		/// </summary>
		internal readonly static HashSet<int> CompletedLevels = new HashSet<int>();

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

			NUM_SEQUENTIAL_LEVELS = SequentialLevels.Length;

			if (doc == null)
				return;

			var roots                 = doc.ChildNodes;
			var configElement         = roots[1] as XmlElement;
			var completedLevelElement = configElement.FindChild(COMPLETED_LEVELS_ELEMENT);

			foreach (var num in completedLevelElement.InnerText.Trim().Split(' '))
			{
				CompletedLevels.Add(Convert.ToInt32(num.Trim()));
			}
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
		}
	}
}
