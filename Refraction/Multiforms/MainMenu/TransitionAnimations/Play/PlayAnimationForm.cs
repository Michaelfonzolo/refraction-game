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
using DemeterEngine.Maths;
using DemeterEngine.Multiforms;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Refraction_V2.Multiforms.ForegroundContent;

using System;
using System.Collections.Generic;

#endregion

namespace Refraction_V2.Multiforms.MainMenu.TransitionAnimations.Play
{

    /// <summary>
    /// The form in charge of the animation that plays when the user presses the
    /// "Play" button in the MainMenuMultiform.
    /// </summary>
    public class PlayAnimationForm : Form
    {

        public static readonly Random Random = new Random();

        // NOTE: Henceforth, the lasers that spawn randomly after the radial lasers
        // in the beginning will be referred to as the "secondary lasers".

        #region Animation Constants

        /// <summary>
        /// The default colour of the secondary lasers.
        /// </summary>
        public static readonly Color SecondaryLaserColour = Color.LightBlue;

        /// <summary>
        /// The number of frames between spawning of individual secondary lasers.
        /// </summary>
        public const int SecondaryLaserSpawnInterval = 1;

        /// <summary>
        /// The start time of secondary laser spawning.
        /// </summary>
        public const int SecondaryLaserSpawnStart = 135;

        /// <summary>
        /// The end time of secondary laser spawning.
        /// </summary>
        public const int SecondaryLaserSpawnEnd = 250;

        public const float SecondaryLaserScale = 0.2f;

        #endregion

        #region Animation Auxiliary Mathematical Functions

        /// <summary>
        /// The function that determines the radius of an individual secondary laser
        /// at a given time in it's lifetime.
        /// </summary>
        public static readonly Func<int, float> SecondaryLaserRadiusFunction
            // The function uses Math.Min to prevent the lasers from stretching farther
            // offscreen than is necessary. The longer the laser, the more computing power
            // it takes to draw it, so we should try to keep it bounded.
            = n => Math.Min(50f * n, 1000);

        /// <summary>
        /// The function that determines the alpha value of the white fade-out over
        /// time.
        /// </summary>
        public static readonly Func<int, float> WhiteOutAlphaFunction = n => (float)Math.Pow(n / 240f, 10);

        #endregion

        /// <summary>
        /// A set of parameters dictating how a burst of radial lasers should look.
        /// </summary>
        private struct RadialLaserSpawnPoint
        {
            /// <summary>
            /// The time at which the lasers spawn.
            /// </summary>
            public int   Time { get; set; }

            /// <summary>
            /// The number of lasers.
            /// </summary>
            public int   Lasers { get; set; }

            /// <summary>
            /// The amount by which to multiply the radial lasers' angle animator function.
            /// </summary>
            public float AngleMul { get; set; }

            /// <summary>
            /// The amount by which to dampen the radial lasers' angle animator function. Here
            /// the term "dampen" means divide the range by a constant. For example, the function
            /// f(x / 3) is f(x) dampened by a factor of 3.
            /// </summary>
            public float AngleDamp    { get; set; }

            /// <summary>
            /// The radius to which the lasers extend in their initial pulse.
            /// </summary>
            public float PulseRadius  { get; set; }

            /// <summary>
            /// The tension with which the lasers are pulled back into the center after the initial
            /// pulse.
            /// </summary>
            public float PulseTension { get; set; }

            /// <summary>
            /// The time of the secondary pulse.
            /// </summary>
            public float SecondaryPulseTime  { get; set; }

            /// <summary>
            /// The amount of recoil the lasers have during their secondary pulse. The smaller this
            /// value, the faster the lasers shoot out the second time.
            /// </summary>
            public float SecondaryPulseRecoil  { get; set; }

            public float Thickness { get; set; }
            public Color Colour { get; set; }
        }

        private static readonly List<RadialLaserSpawnPoint> SpawnPoints
            = new List<RadialLaserSpawnPoint>()
        {
            new RadialLaserSpawnPoint{ 
                Time = 1, Lasers = 4, AngleMul = 0.5f, AngleDamp = 100f, PulseRadius = 250f, PulseTension = 0.8f, 
                SecondaryPulseTime = 180f, SecondaryPulseRecoil = 0.1f, Thickness = 1f, Colour = Color.Blue },
            new RadialLaserSpawnPoint{ 
                Time = 30, Lasers=6, AngleMul = -1.2f, AngleDamp = 100f, PulseRadius = 267f, PulseTension = 1.2f, 
                SecondaryPulseTime = 150f, SecondaryPulseRecoil = 0.2f, Thickness = 1f, Colour = Color.Green },
            new RadialLaserSpawnPoint{ 
                Time = 65, Lasers = 10, AngleMul = 1.5f, AngleDamp = 50f, PulseRadius = 284f, PulseTension = 1.6f, 
                SecondaryPulseTime = 130f, SecondaryPulseRecoil = 0.4f, Thickness = 1f, Colour = Color.Red },
            new RadialLaserSpawnPoint{ 
                Time = 100, Lasers = 20, AngleMul = -2f, AngleDamp = 50f,  PulseRadius = 300f, PulseTension = 2f, 
                SecondaryPulseTime = 120f, SecondaryPulseRecoil = 0.5f, Thickness = 1f, Colour = Color.Magenta }
        };

        /// <summary>
        /// The center of the animation.
        /// </summary>
        public Vector2 Center { get; private set; }

        /// <summary>
        /// The list of all currently active lasers.
        /// </summary>
        private List<AnimatedLaserSegment> Lasers = new List<AnimatedLaserSegment>();

        /// <summary>
        /// The list of all currently active particles.
        /// </summary>
        private List<ClickParticle> Particles = new List<ClickParticle>();

        public PlayAnimationForm(Vector2 center) 
            : base(true) 
        {
            Center = center;
        }

        /// <summary>
        /// Add lasers according to the parameters specified by a spawn point.
        /// </summary>
        /// <param name="spawnPoint"></param>
        private void AddLasers(RadialLaserSpawnPoint spawnPoint)
        {
            var n               = spawnPoint.Lasers;
            var angleMultiplier = spawnPoint.AngleMul;
            var angleDampener   = spawnPoint.AngleDamp;
            var pulseRadius     = spawnPoint.PulseRadius;
            var pulseTension    = spawnPoint.PulseTension;
            var pulseRepeat     = spawnPoint.SecondaryPulseTime;
            var pulseRecoil     = spawnPoint.SecondaryPulseRecoil;
            var thickness       = spawnPoint.Thickness;
            var colour          = spawnPoint.Colour;

            var angleIncrement = 360f / n;
            for (int i = 0; i < n; i++)
            {
                var laser = new RadialAnimatedLaser(Center, angleIncrement * i, 1, colour, thickness);
                Func<int, float> animator = new LaserRadiusAnimationFunction(
                    y1: pulseRadius, x2: LaserRadiusAnimationFunction.DEFAULT_X1 + pulseTension, 
                    x3: pulseRepeat, x4: pulseRepeat + pulseRecoil).Func;

                laser.SetRadiusAnimator(animator, false);
                laser.SetAngleAnimator(m => angleMultiplier * (float)SpecialFunctions.J0(m / angleDampener));

                Lasers.Add(laser);
            }

            var count = Random.Next(80, 100);
            for (int i = 0; i < count; i++)
            {
                var velocity = (5f * (float)Random.NextDouble() + 1f) * VectorUtils.Polar(360 * Random.NextDouble());
                var lifetime = Random.Next(40, 100);
                Particles.Add(new ClickParticle(Center, velocity, lifetime, colour));
            }
        }

        private void AddRandomLaser()
        {
            var laser = new RadialAnimatedLaser(
                Center, 360 * (float)Random.NextDouble(), 1, SecondaryLaserColour, SecondaryLaserScale);
            laser.SetRadiusAnimator(SecondaryLaserRadiusFunction, false);
            Lasers.Add(laser);
        }

        public override void Update()
        {
            base.Update();

            foreach (var spawnPoint in SpawnPoints)
            {
                if (AtFrame(spawnPoint.Time))
                {
                    AddLasers(spawnPoint);
                }
            }

            if (AtFrameIntervals(SecondaryLaserSpawnInterval, SecondaryLaserSpawnStart, SecondaryLaserSpawnEnd))
            {
                AddRandomLaser();
            }

            foreach (var laser in Lasers)
            {
                laser.Update();
            }

            foreach (var particle in Particles)
            {
                particle.Update();
            }
            Particles.RemoveAll(p => p.Dead);
        }

        public override void Render()
        {
            foreach (var laser in Lasers)
            {
                laser.Render();
            }

            DisplayManager.SetSpriteBatchProperties(blendState: BlendState.Additive);

            foreach (var particle in Particles)
            {
                particle.Render();
            }

            DisplayManager.ClearSpriteBatchProperties();

            DisplayManager.SetSpriteBatchProperties(blendState: BlendState.NonPremultiplied);

            var dummyTexture = new Texture2D(DisplayManager.GraphicsDevice, 1, 1);
            dummyTexture.SetData(new Color[] { Color.White });

            var color = new Color(Color.White, WhiteOutAlphaFunction(LocalFrame));
            DisplayManager.Draw(
                dummyTexture, Vector2.Zero, null, color, 0f, Vector2.Zero,
                new Vector2(DisplayManager.WindowWidth, DisplayManager.WindowHeight),
                SpriteEffects.None, 0f);

            DisplayManager.ClearSpriteBatchProperties();
        }

    }
}
