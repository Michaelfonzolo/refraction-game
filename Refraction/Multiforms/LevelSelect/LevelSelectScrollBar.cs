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

using DemeterEngine;
using DemeterEngine.Multiforms.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Refraction_V2.Multiforms.LevelSelect
{
	public class LevelSelectScrollBar : ScrollBarForm
	{

		public LevelSelectScrollBar(
			Vector2 topLeft, int scrollBarLength, int spanLength, 
			int scrollBarWidth, bool vertical, bool keepsTime = true)

			: base(topLeft, scrollBarLength, spanLength, scrollBarWidth, vertical, keepsTime)
		{

		}

        public override void Render()
        {
            var dummyTexture = new Texture2D(DisplayManager.GraphicsDevice, 1, 1);
            dummyTexture.SetData(new Color[] { Color.White });

            DisplayManager.Draw(
                dummyTexture, ScrollBarCollider.TopLeft, null, Color.DarkGray,
                0f, Vector2.Zero, new Vector2((float)ScrollBarCollider.W, (float)ScrollBarCollider.H),
                SpriteEffects.None, 0f);

            DisplayManager.Draw(
                dummyTexture, ScrollThumbCollider.TopLeft, null, ThumbCollidingWithMouse ? Color.Red : Color.White, 
                0f, Vector2.Zero, new Vector2((float)ScrollThumbCollider.W, (float)ScrollThumbCollider.H), 
                SpriteEffects.None, 0f);

        }

	}
}
