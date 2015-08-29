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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

#endregion

namespace Refraction_V2.Multiforms.ForegroundContent
{

    /// <summary>
    /// A particle that spawns when the user clicks.
    /// 
    /// This object is also used for other non-click related particle effects.
    /// </summary>
    public class ClickParticle : ChronometricObject
    {

        /// <summary>
        /// The position of the particle.
        /// </summary>
        public Vector2 Position { get; private set; }

        /// <summary>
        /// The particle's velocity.
        /// </summary>
        public Vector2 Velocity { get; private set; }

        /// <summary>
        /// The number of frames before the particle is removed.
        /// </summary>
        public int LifeTime { get; private set; }

        /// <summary>
        /// The colour of the particle.
        /// </summary>
        public Color Colour { get; private set; }

        /// <summary>
        /// Indicates whether or not to remove the particle from the game world.
        /// </summary>
        public bool Dead { get { return LocalFrame > LifeTime; } }

        public ClickParticle(Vector2 pos, Vector2 velocity, int lifeTime, Color colour) : base(true)
        {
            Position = pos;
            Velocity = velocity;
            LifeTime = lifeTime;
            Colour = colour;
        }

        /// <summary>
        /// The function that calculates the speed of the particle at a given time.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private float SpeedFunction(int n)
        {
            return 1.5f - LocalFrame / (float)LifeTime;
        }

        /// <summary>
        /// The function that calculates the alpha of the particle at a given time.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private float AlphaFunction(int n)
        {
            return 1f - n / (float)LifeTime;
        }

        public void Update()
        {
            UpdateTime();
            Position += SpeedFunction(LocalFrame) * Velocity;
        }

        public void Render()
        {
            var texture = Assets.Shared.Images.LaserCap;
            var offset = new Vector2(texture.Width, texture.Height / 2f);

            var alpha = AlphaFunction(LocalFrame);
            var col1 = new Color(Colour, alpha);
            var col2 = new Color(Color.White, alpha);

            DisplayManager.Draw(texture, Position, null, col1, 0f, offset, 0.3f, SpriteEffects.None, 0f);
            DisplayManager.Draw(texture, Position, null, col1, (float)Math.PI, offset, 0.3f, SpriteEffects.None, 0f);

            DisplayManager.Draw(texture, Position, null, col2, 0f, offset, 0.2f, SpriteEffects.None, 0f);
            DisplayManager.Draw(texture, Position, null, col2, (float)Math.PI, offset, 0.2f, SpriteEffects.None, 0f);
        }
    }
}
