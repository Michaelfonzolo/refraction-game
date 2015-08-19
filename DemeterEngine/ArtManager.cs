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
 * A singleton representing a global Content manager, which can load different
 * art assets from the content folder specified in the game's .gamesetup file.
 * (For more information see GameSetupReader).
 */

#endregion

#region Using Statements

using DemeterEngine.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

#endregion

namespace DemeterEngine
{
    public class ArtManager
    {

        private ArtManager() { }

		/// <summary>
		/// The content manager the art manager uses to load objects.
		/// </summary>
        private ContentManager Content;

		/// <summary>
		/// The singleton instance of the art manager.
		/// </summary>
        private static ArtManager Instance = new ArtManager();

		// These need to be internal to prevent objects in external assemblies from
		// initializing or unloading the art manager at invalid times.

        internal static void Initialize(ContentManager content)
        {
            if (Instance.Content != null)
                throw new ArgumentException("A content manager has already been supplied to the ArtManager.");
            Instance.Content = content;
        }

        internal static void Unload()
        {
            Instance.Content.Unload();
            Instance.Content.Dispose();
        }

		/// <summary>
		/// Load a Texture2D from a given file name.
		/// </summary>
        public static Texture2D Texture2D(string fileName)
        {
            return Instance.Content.Load<Texture2D>(fileName);
        }

		/// <summary>
		/// Load a Sprite from a given file name.
		/// </summary>
        public static Sprite Sprite(string fileName)
        {
            return new Sprite(Instance.Content.Load<Texture2D>(fileName));
        }

		/// <summary>
		/// Load a SpriteFont from a given file name.
		/// </summary>
		public static SpriteFont Font(string fileName)
		{
			return Instance.Content.Load<SpriteFont>(fileName);
		}

    }
}
