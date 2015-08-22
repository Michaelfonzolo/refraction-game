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
 * The LevelNameInfo includes information on the currently loaded level's name.
 * 
 * The reason I decided to use a level name info class instead of just a string representing
 * the name of the .levelfile to load is because a level can either be part of the sequential
 * listing of classes, in which case the user can navigate back and forth throughout it, or it
 * can be a standalone file the user wants to load.
 */

#endregion

#region Using Statements

using System;

#endregion

namespace Refraction_V2.Multiforms.Level
{
	public class LevelNameInfo
	{
		private string _levelName;

		public string LevelName
		{
			get
			{
				if (Sequential)
					return LoadedLevelManager.SequentialLevels[LevelNumber.Value];
				return _levelName;
			}
		}


		/// <summary>
		/// Whether or not this level is part of the sequential listing of levels,
		/// or is just the name of a file to load.
		/// </summary>
		public bool Sequential { get { return LevelNumber.HasValue; } }

		public int? LevelNumber { get; private set; }

		public LevelNameInfo(string levelName) 
		{
			_levelName = levelName;
		}

		public LevelNameInfo(int levelNum)
		{
			if (0 > levelNum || levelNum > LoadedLevelManager.SequentialLevels.Length)
				throw new LevelLoadException(
					String.Format("Invalid level number {0}.", levelNum));
			LevelNumber = levelNum;
		}

		/// <summary>
		/// Check if there is a previous level in the sequential listing of levels.
		/// </summary>
		/// <returns></returns>
		public bool HasPrevLevel()
		{
			return Sequential && LevelNumber > 0;
		}

		/// <summary>
		/// Check if there is a next level in the sequential listing of levels.
		/// </summary>
		/// <returns></returns>
		public bool HasNextLevel()
		{
			return Sequential && LevelNumber < LoadedLevelManager.SequentialLevels.Length - 1;
		}

		/// <summary>
		/// Increment the level number. This assumes you have already called HasNextLevel()
		/// and received a value of 'true', otherwise calling this method will fail.
		/// </summary>
		public void IncrementLevel()
		{
			LevelNumber++;
		}

		/// <summary>
		/// Decrement the level number. This assumes you have already called HasPrevLevel()
		/// and received a value of 'true', otherwise calling this method will fail.
		/// </summary>
		public void DecrementLevel()
		{
			LevelNumber--;
		}

	}
}
