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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

#endregion

namespace Refraction_V2.Multiforms
{

    /// <summary>
    /// An arbitrary laser segment which can be rendered.
    /// </summary>
    public class LaserSegment : ChronometricObject
    {
        /// <summary>
        /// The start position of the laser segment.
        /// </summary>
        public Vector2 Start { get; protected set; }

        /// <summary>
        /// The end position of the laser segment.
        /// </summary>
        public Vector2 End { get; protected set; }

        /// <summary>
        /// The colour of the laser.
        /// </summary>
        public Color Colour { get; protected set; }

        public LaserSegment(Vector2 start, Vector2 end, Color color, bool keepTime = false)
            : base(keepTime)
        {
            Start = start;
            End = end;
            Colour = color;
        }

        public void Render(float scale)
        {
            // This code almost directly ripped from
            // http://gamedevelopment.tutsplus.com/tutorials/how-to-generate-shockingly-good-2d-lightning-effects--gamedev-2681

            var cap = Assets.Shared.Images.LaserCap;
            var segmentTexture = Assets.Shared.Images.LaserSegment;

            var tangent = End - Start;
            var rotation = (float)Math.Atan2(tangent.Y, tangent.X);

            var capOrigin = new Vector2(cap.Width, cap.Height / 2f);
            var middleOrigin = new Vector2(0, segmentTexture.Height / 2f);
            var middleScale = new Vector2(tangent.Length(), scale);

            DisplayManager.SetSpriteBatchProperties(blendState: BlendState.Additive);

            var i = 1;
            foreach (var color in new Color[] { Colour, Color.White })
            {
                DisplayManager.Draw(
                    segmentTexture, Start, null, color,
                    rotation, middleOrigin, new Vector2(middleScale.X, middleScale.Y / i), SpriteEffects.None, 0f);

                DisplayManager.Draw(
                    cap, Start, null, color,
                    rotation, capOrigin, scale / i, SpriteEffects.None, 0f);

                DisplayManager.Draw(
                    cap, End, null, color,
                    rotation + (float)Math.PI, capOrigin, scale / i,
                    SpriteEffects.None, 0f);
                i++;
            }

            DisplayManager.ClearSpriteBatchProperties();
        }
    }
}
