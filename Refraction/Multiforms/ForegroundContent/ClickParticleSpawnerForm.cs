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
using DemeterEngine.Maths;
using DemeterEngine.Multiforms;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Refraction_V2.Multiforms.Level;

using System;
using System.Collections.Generic;

#endregion

namespace Refraction_V2.Multiforms.ForegroundContent
{

    /// <summary>
    /// A form that spawns particles when the user clicks.
    /// </summary>
    public class ClickParticleSpawnerForm : Form
    {

        #region Constants

        public static readonly Random Random = new Random();

        /// <summary>
        /// The maximum number of particles on screen at once. To achieve this
        /// maximum the user would have to click at least 11 times a second, which
        /// is nearly impossible, but the cap exists to prevent bug-related 
        /// particle spamming.
        /// </summary>
        public const int MAX_PARTICLES = 200;

        private const float VELOCITY_MULTIPLIER = 2f;

        private const float MIN_VELOCITY = 0.1f;

        /// <summary>
        /// The colours that the particles can be. They correspond to the
        /// (most common) laser colours.
        /// </summary>
        public static readonly List<LaserColours> Colours = new List<LaserColours>()
        {
            LaserColours.Red,
            LaserColours.Blue,
            LaserColours.Green,
            LaserColours.Orange,
            LaserColours.Purple,
            LaserColours.Cyan,
        };

        #endregion

        /// <summary>
        /// The list of currently active particles.
        /// </summary>
        public List<ClickParticle> Particles = new List<ClickParticle>();

        /// <summary>
        /// The current colour. Every time the user clicks, the colour that the particles
        /// take switches.
        /// </summary>
        private int currentColour = 0;

        private Vector2 GetRandomVelocity()
        {
            return (VELOCITY_MULTIPLIER * (float)Random.NextDouble() + MIN_VELOCITY) * 
                   VectorUtils.Polar(360f * Random.NextDouble());
        }

        public override void Update()
        {
            base.Update();

            if (MouseInput.IsClicked(MouseButtons.Left) || KeyboardInput.IsClicked(Keys.Z) || KeyboardInput.IsClicked(Keys.X))
            {
                var col = Colours[currentColour];
                var pos = MouseInput.MouseVector;
                var particleCount = Random.Next(10, 20);
                for (int i = 0; i < particleCount; i++)
                {
                    if (Particles.Count == MAX_PARTICLES)
                    {
                        break;
                    }
                    var vel = GetRandomVelocity();
                    var lifetime = Random.Next(20, 50);
                    Particles.Add(new ClickParticle(pos, vel, lifetime, col.Color));
                }

                currentColour++;
                currentColour %= Colours.Count;
            }

            foreach (var particle in Particles)
            {
                particle.Update();
            }

            Particles.RemoveAll(p => p.Dead);
        }

        public override void Render()
        {
            DisplayManager.SetSpriteBatchProperties(blendState: BlendState.Additive);

            foreach (var particle in Particles)
            {
                particle.Render();
            }

            DisplayManager.ClearSpriteBatchProperties();
        }
    }
}
