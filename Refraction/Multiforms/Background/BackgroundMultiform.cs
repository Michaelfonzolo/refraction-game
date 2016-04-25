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
using DemeterEngine.Multiforms;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.IO;

#endregion

namespace Refraction_V2.Multiforms.Background
{
    public class BackgroundMultiform : RefractionGameMultiform
    {

        #region Form Name Constants

        /// <summary>
        /// The name of this multiform.
        /// </summary>
        public const string MultiformName = "Background";

        #endregion

        #region Auxiliary Math Constants and Functions

        private const float BG_ALPHA = 0.5f;

        private const float BG_SCALE = 0.8f;

        private const float INIT_OFFSET_X = -300f;

        private const float INIT_OFFSET_Y = -30f;

        private const float OFFSET_STRETCH = 775f;

        private const float OFFSET_AMPLITUDE = 30f;

        private const float ROTATION_STRETCH = 250f;

        private const float ROTATION_AMPLITUDE = 0.02f;

        private static readonly Func<int, float> OffsetSineSquared =
            n => 2 * (float)Math.Pow(Math.Sin(n / ROTATION_STRETCH), 2) - 1;

        /// <summary>
        /// The function that measures the offset of the background over time.
        /// </summary>
        private static readonly Func<int, Vector2> OffsetFunction =
            n => new Vector2(
                    INIT_OFFSET_X + OFFSET_AMPLITUDE * OffsetSineSquared(n), 
                    INIT_OFFSET_Y
                    );

        /// <summary>
        /// The function of the background rotation over time.
        /// </summary>
        private static readonly Func<int, float> RotationFunction = 
            n => (float)Math.Sin(n / ROTATION_STRETCH) * ROTATION_AMPLITUDE;

        #endregion

        /// <summary>
        /// The texture of the background.
        /// </summary>
        public Texture2D BackgroundTexture;

        public override void Construct(MultiformTransmissionData args)
        {
            SetUpdater(Update_Main);
            SetRenderer(Render_Main);
            BackgroundTexture = Assets.Shared.Images.Background;
        }

        public void Update_Main()
        {
            UpdateTime();
        }

        public void FadeAlpha(float scale)
        {

        }

        public void Render_Main()
        {
            var pos = OffsetFunction(LocalFrame);
            var rotation = RotationFunction(LocalFrame);

            /*
            DisplayManager.Draw(
                BackgroundTexture, pos, null, Color.White * BG_ALPHA, 
                rotation, Vector2.Zero, BG_SCALE, SpriteEffects.None, 0f
                );
             */
        }

    }
}
