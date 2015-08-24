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
using DemeterEngine.Collision;
using DemeterEngine.Graphics;
using DemeterEngine.Input;
using DemeterEngine.Multiforms.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#endregion

namespace Refraction_V2.Multiforms.LevelSelect
{
	public class LevelSelectButton : ButtonForm
	{

        /// <summary>
        /// The width of the button sprite.
        /// </summary>
		public const int BUTTON_WIDTH = 50;

        /// <summary>
        /// The height of the button sprite.
        /// </summary>
		public const int BUTTON_HEIGHT = 50;

        /// <summary>
        /// The sprite representing the button.
        /// </summary>
		public Sprite ButtonSprite { get; private set; }

        /// <summary>
        /// The level number font.
        /// </summary>
		public SpriteFont LevelNoFont { get; private set; }

        /// <summary>
        /// The level number this button represents.
        /// </summary>
		public int LevelNo { get; private set; }

        /// <summary>
        /// A reference to the parent multiform's scroll bar form.
        /// </summary>
        private LevelSelectScrollBar ScrollBar;

        /// <summary>
        /// The position of the text.
        /// </summary>
		public Vector2 TextPosition { get; private set; }


        // This is the stuff related to the function that calculates the alpha of a button
        // relative to it's height on the screen.
        #region Alpha Function


        /* The alpha function looks like this. The sections are outlined below.
         * 
         * 1        __________________
         *         /                  \
         *        /                    \
         *       /                      \
         * 0 ___/                        \_____
         * 
         *   -/ \ / \----------------/ \ / \---
         *   |   |           |          |    |
         *   |   |           |          |   > WINDOW_HEIGHT - 100
         *   |   |           |          |
         *   |   |           |    WINDOW_HEIGHT - 200 -> WINDOW_HEIGHT - 100
         *   |   |           |
         *   |   |      INITIAL_LEVEL_SELECT_Y_OFFSET -> WINDOW_HEIGHT - 200
         *   |   |
         *   |   0 -> INITIAL_LEVEL_SELECT_Y_OFFSET
         *   |
         *   < 0
         *
         */
    

        private static double GetAlpha(double yPos)
        {
            if (yPos < -BUTTON_HEIGHT || yPos > 768 - 100)
                return 0d;
            else if (yPos < 40)
                return (yPos + 50) / 90d;
            else if (yPos < 768 - 200)
                return 1d;
            else if (yPos < 768 - 100)
                return (768 - 100 - yPos) / 100d;
            return 0;
        }

        #endregion

		public LevelSelectButton(RectCollider collider, LevelSelectScrollBar scrollBar, int levelNo)
			: base(collider)
		{
			LevelNo = levelNo;
            ScrollBar = scrollBar;

            if (LoadedLevelManager.CompletedLevels.Contains(levelNo))
            {
                ButtonSprite = new Sprite(Assets.LevelSelect.Images.ClearedLevelButton);
            }
            else
            {
                ButtonSprite = new Sprite(Assets.LevelSelect.Images.UnclearedLevelButton);
            }
			LevelNoFont  = Assets.LevelSelect.Fonts.LevelNo;

			ButtonSprite.Position = collider.TopLeft;
			var textDimensions = LevelNoFont.MeasureString(LevelNo.ToString());
			TextPosition = (new Vector2(BUTTON_WIDTH, BUTTON_HEIGHT) - textDimensions) / 2f + collider.TopLeft;
		}

        public override void Update()
        {
            if (GetAlpha(((RectCollider)Collider).Top) < 0.5)
            {
                LockInteraction();
            }
            else
            {
                UnlockInteraction();
            }

            base.Update();

            if (IsReleased(MouseButtons.Left))
            {
                ((LevelSelectMultiform)Parent).ButtonPressed(LevelNo);
            }

            if (ScrollBar == null)
                return;

            var offset = -(float)ScrollBar.DeltaSpanScrollValue + 0.25f * (float)Math.Sin(LocalFrame / 20f);
            ((RectCollider)Collider).Translate(0, offset);
            TextPosition += new Vector2(0, offset);
        }

        public override void Render()
		{
            var position = ((RectCollider)Collider).TopLeft;

            ButtonSprite.Tint = CollidingWithMouse ? Color.Red : Color.White;
            ButtonSprite.Alpha = 255 * GetAlpha(position.Y);

			ButtonSprite.Render(position);
			DisplayManager.DrawString(LevelNoFont, (LevelNo + 1).ToString(), TextPosition, Color.Black);
		}
	}
}
