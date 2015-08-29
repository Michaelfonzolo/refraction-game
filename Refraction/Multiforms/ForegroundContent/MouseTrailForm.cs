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
using System.Collections.Generic;

#endregion

namespace Refraction_V2.Multiforms.ForegroundContent
{
    public class MouseTrailForm : Form
    {

        /// <summary>
        /// The maximum number of trail points to store.
        /// </summary>
        public const int MAX_POINTS = 50;

        /// <summary>
        /// The texture to use for a single segment of the trail.
        /// </summary>
        public static readonly Texture2D TrailSeg = Assets.Shared.Images.MouseTrailSegment;

        /// <summary>
        /// The function which calculates the scale of the "i"th segment in the trail.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private float ScaleFunc(int i)
        {
            return i / (float)TrailPoints.Count;
        }

        /// <summary>
        /// The function which calculates the alpha of the "i"th segment in the trail.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private float AlphaFunc(int i)
        {
            return hidden ? 0f : 0.4f * i / (float)TrailPoints.Count;
        }

        /// <summary>
        /// The list of points in the trail.
        /// </summary>
        public List<Vector2> TrailPoints = new List<Vector2>();

        /// <summary>
        /// The previous point in the trail.
        /// </summary>
        private Vector2 PrevPoint = Vector2.Zero;

        /// <summary>
        /// Whether or not the trail is visible.
        /// </summary>
        private bool hidden = false;

        public MouseTrailForm()
            : base(true)
        {

        }

        public override void Update()
        {
            base.Update();

            // The following if-else statements makes it so that if the player 
            // is not moving the mouse the trail "catches up" with the mouse
            // until there are no points left in the trail.

            var newPoint = MouseInput.MouseVector;
            if (newPoint != PrevPoint)
            {
                TrailPoints.Add(newPoint);
                if (TrailPoints.Count > MAX_POINTS)
                {
                    TrailPoints.RemoveAt(0);
                }
                PrevPoint = newPoint;
            }
            else
            {
                if (TrailPoints.Count > 0)
                {
                    TrailPoints.RemoveAt(0);
                }
            }
        }

        public void Hide()
        {
            hidden = true;
        }

        public void Unhide()
        {
            hidden = false;
        }

        public override void Render()
        {
            if (TrailPoints.Count < 2)
                return;

            DisplayManager.SetSpriteBatchProperties(
                sortMode: SpriteSortMode.Texture, blendState: BlendState.NonPremultiplied);

            var total = TrailPoints.Count;
            for (int i = 0; i < total - 1; i++)
            {
                RenderSegment(TrailPoints[i], TrailPoints[i + 1], ScaleFunc(i), AlphaFunc(i));
            }

            DisplayManager.ClearSpriteBatchProperties();
        }

        private void RenderSegment(Vector2 v1, Vector2 v2, float scale, float alpha)
        {
            var tangent = v2 - v1;
            var rotation = (float)Math.Atan2(tangent.Y, tangent.X);

            var middleOrigin = new Vector2(0, TrailSeg.Height / 2f);
            var middleScale = new Vector2(tangent.Length(), scale);

            var color = new Color(Color.White, alpha);

            DisplayManager.Draw(
                TrailSeg, v1, null, color,
                rotation, middleOrigin, new Vector2(middleScale.X, middleScale.Y), SpriteEffects.None, 0f);
        }

    }
}
