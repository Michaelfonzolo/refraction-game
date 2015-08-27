using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DemeterEngine;
using DemeterEngine.Maths;
using DemeterEngine.Multiforms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Refraction_V2.Multiforms.MainMenu.TransitionAnimations.Play
{
    public class PlayAnimationForm : Form
    {

        public static readonly Random Random = new Random();

        #region Animation Constants

        public static readonly Color LaserColor1 = Color.Blue;

        public static readonly Color LaserColor2 = Color.LightBlue;

        public const int RandomLaserSpawnInterval = 4;

        public const int RandomLaserSpawnStart = 100;

        public const int RandomLaserSpawnEnd = 230;

        public const float SecondaryLaserScale = 0.2f;

        #endregion

        #region Animation Auxiliary Mathematical Functions

        public static readonly Func<int, float> SecondaryLaserRadiusFunction = n => 50f * n;

        public static readonly Func<int, float> WhiteOutAlphaFunction = n => (float)Math.Pow(n / 200f, 6);

        #endregion

        private struct RadialLaserSpawnPoint
        {
            public int   Time      { get; set; }
            public int   Lasers    { get; set; }
            public float AngleMul  { get; set; }
            public float AngleDamp { get; set; }
        }

        private static readonly List<RadialLaserSpawnPoint> SpawnPoints
            = new List<RadialLaserSpawnPoint>()
        {
            new RadialLaserSpawnPoint{ Time = 1,  Lasers = 4,  AngleMul = 0.5f,  AngleDamp = 100f },
            new RadialLaserSpawnPoint{ Time = 20, Lasers = 6,  AngleMul = -1.2f, AngleDamp = 100f },
            new RadialLaserSpawnPoint{ Time = 45, Lasers = 10, AngleMul = 1.5f,  AngleDamp = 50f },
            new RadialLaserSpawnPoint{ Time = 75, Lasers = 25, AngleMul = 2f,    AngleDamp = 50f }
        };

        public Vector2 Center { get; private set; }

        private List<AnimatedLaserSegment> Lasers = new List<AnimatedLaserSegment>();

        public PlayAnimationForm(Vector2 center) 
            : base(true) 
        {
            Center = center;
        }

        private void AddLasers(int n, float angleMultiplier, float angleDampener)
        {
            var angleIncrement = 360f / n;
            for (int i = 0; i < n; i++)
            {
                var laser = new RadialAnimatedLaser(Center, angleIncrement * i, 1, LaserColor1);

                laser.SetRadiusAnimator(new LaserRadiusAnimation().Func, false);
                laser.SetAngleAnimator(m => angleMultiplier * (float)SpecialFunctions.J0(m / angleDampener));

                Lasers.Add(laser);
            }
        }

        private void AddRandomLaser()
        {
            var laser = new RadialAnimatedLaser(
                Center, 360 * (float)Random.NextDouble(), 1, LaserColor2, SecondaryLaserScale);
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
                    AddLasers(spawnPoint.Lasers, spawnPoint.AngleMul, spawnPoint.AngleDamp);
                }
            }

            if (AtFrameIntervals(RandomLaserSpawnInterval, RandomLaserSpawnStart, RandomLaserSpawnEnd))
            {
                AddRandomLaser();
            }

            foreach (var laser in Lasers)
            {
                laser.Update();
            }
        }

        public override void Render()
        {
            foreach (var laser in Lasers)
            {
                laser.Render();
            }

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
