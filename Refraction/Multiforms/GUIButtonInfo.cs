﻿#region License

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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Refraction_V2.Multiforms
{

    /// <summary>
    /// Information regarding how to construct a GUIButton.
    /// </summary>
    public class GUIButtonInfo
    {

        /// <summary>
        /// The text to display on the button.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// The texture of the button.
        /// </summary>
        public Texture2D Texture { get; private set; }

        /// <summary>
        /// The font in which to display the text.
        /// </summary>
        public SpriteFont Font { get; private set; }

        /// <summary>
        /// The (initial) colour of the text.
        /// </summary>
        public Color? InitialTextColour { get; set; }

        public GUIButtonInfo(string text, Texture2D texture)
            : this(text, texture, Assets.Shared.Fonts.GUIButtonFont_Medium) { }

        public GUIButtonInfo(string text, Texture2D texture, SpriteFont font)
        {
            Text = text;
            Texture = texture;
            Font = font;
            InitialTextColour = null;
        }
    }
}
