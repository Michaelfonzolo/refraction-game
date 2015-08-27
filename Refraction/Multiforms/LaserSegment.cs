using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DemeterEngine;

namespace Refraction_V2.Multiforms
{
    public class LaserSegment : ChronometricObject
    {
        public Vector2 Start { get; protected set; }

        public Vector2 End { get; protected set; }

        public Color Color { get; protected set; }

        public LaserSegment(Vector2 start, Vector2 end, Color color, bool keepTime = false)
            : base(keepTime)
        {
            Start = start;
            End = end;
            Color = color;
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
            foreach (var color in new Color[] { Color, Color.White })
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
