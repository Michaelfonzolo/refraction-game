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

/* 
 * Author: Michael Ala
 */

#endregion

#region Using Statements

using DemeterEngine;
using DemeterEngine.Input;
using DemeterEngine.Multiforms;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

#endregion

namespace Refraction_V2.Multiforms.ForegroundContent
{

    /// <summary>
    /// A form representing the mouse cursor.
    /// </summary>
    public class MouseCursorForm : Form
    {

        #region Constants

        public static readonly Texture2D CursorTexture = Assets.Shared.Images.MouseCursor;

        /// <summary>
        /// The offset from the mouse position at which to draw the cursor texture.
        /// </summary>
        public static readonly Vector2 Offset = new Vector2(CursorTexture.Width, CursorTexture.Height) / 2f;

        // These constants are a little weird, because the alpha function works like this:
        // we increment the non-static "scale" property by the SCALE_INCREMENT, then multiply
        // the result by SCALE_AMPLITUDE, and add MIN_SCALE. This results in a function where
        // a scale value of zero results in an actual scale value of MIN_SCALE, and a scale
        // value of one results in an actual scale value of MIN_SCALE + SCALE_AMPLITUDE.
        //
        // The confusion comes from thinking the non-static "scale" property actually represents
        // the scale of the texture, when it just represents the variable that is fed into the
        // scale function.

        /// <summary>
        /// The minimum scale value.
        /// </summary>
        private const float MIN_SCALE = 0.75f;

        /// <summary>
        /// The amplitude of the scale function (= max - min).
        /// </summary>
        private const float SCALE_AMPLITUDE = 0.25f;
        
        /// <summary>
        /// The amount by which to increment the scale value.
        /// </summary>
        private const float SCALE_INCREMENT = 0.15f;

        /// <summary>
        /// The function that takes in the non-static "scale" property and returns the
        /// actual scale of the texture.
        /// </summary>
        private static readonly Func<float, float> ScaleFunc =
            scale => SCALE_AMPLITUDE * (float)Math.Pow(Math.Sin(Math.PI / 2 * scale), 2.6) + MIN_SCALE;

        #endregion

        /// <summary>
        /// The parameter to feed into the ScaleFunc to determine the scale of the cursor texture.
        /// </summary>
        private float scale = 0f;

        /// <summary>
        /// The alpha of the mouse cursor texture.
        /// </summary>
        private float alpha = 1f;

        /// <summary>
        /// Whether or not we are fading out.
        /// </summary>
        private bool fadingOut = false;

        /// <summary>
        /// Whether or not we are fading in.
        /// </summary>
        private bool fadingIn = false;

        public MouseCursorForm() : base(true) { }

        private bool PerformFade(float increment, bool condition, float finalVal)
        {
            if (condition)
            {
                alpha = finalVal;
                return false;
            }
            alpha += increment;
            return true;
        }

        public override void Update()
        {
            base.Update();
            if (MouseInput.IsPressed(MouseButtons.Left))
            {
                scale += SCALE_INCREMENT;
            }
            else
            {
                scale -= SCALE_INCREMENT;
            }
            scale = MathHelper.Clamp(scale, 0f, 1f);

            // These don't have to be separate if statements since one or the other will
            // only ever be active, they will never be active simultaneously.
            if (fadingOut)
            {
                fadingOut = PerformFade(-0.05f, alpha <= 0f, 0f);
            }
            else if (fadingIn)
            {
                fadingIn = PerformFade(0.05f, alpha >= 1f, 1f);
            }

            if (MouseInput.IsIdle(200))
            {
                FadeOut();
            }
            else
            {
                FadeIn();
            }
        }

        /// <summary>
        /// Fade the mouse cursor out.
        /// </summary>
        public void FadeOut()
        {
            fadingOut = true;
            fadingIn = false;
        }

        /// <summary>
        /// Fade the mouse cursor in.
        /// </summary>
        public void FadeIn()
        {
            fadingOut = false;
            fadingIn = true;
        }

        public override void Render()
        {
            base.Render();

            DisplayManager.SetSpriteBatchProperties(blendState: BlendState.NonPremultiplied);

            var colour = new Color(Color.White, alpha);
            DisplayManager.Draw(
                CursorTexture, MouseInput.MouseVector, null, colour, 0f, 
                Offset, ScaleFunc(scale), SpriteEffects.None, 0f);

            DisplayManager.ClearSpriteBatchProperties();
        }
    }
}
