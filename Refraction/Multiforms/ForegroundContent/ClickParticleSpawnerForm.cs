using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DemeterEngine;
using DemeterEngine.Graphics;
using DemeterEngine.Input;
using DemeterEngine.Maths;
using DemeterEngine.Multiforms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Refraction_V2.Multiforms.Level;

namespace Refraction_V2.Multiforms.ForegroundContent
{
    public class ClickParticleSpawnerForm : Form
    {

        public static readonly Random Random = new Random();

        public const int MAX_PARTICLES = 500;

        public List<ClickParticle> Particles = new List<ClickParticle>();

        public List<LaserColours> Colours = new List<LaserColours>()
        {
            LaserColours.Red,
            LaserColours.Blue,
            LaserColours.Green,
            LaserColours.Yellow,
            LaserColours.Orange,
            LaserColours.Purple,
            LaserColours.Cyan,
            LaserColours.White
        };

        private int currentColour = 0;

        public override void Update()
        {
            base.Update();

            if (MouseInput.IsClicked(MouseButtons.Left))
            {
                var col = Colours[currentColour];
                var pos = MouseInput.MouseVector;
                var randint = Random.Next(10, 20);
                for (int i = 0; i < randint; i++)
                {
                    if (Particles.Count == MAX_PARTICLES)
                    {
                        break;
                    }
                    var vel = (2f * (float)Random.NextDouble() + 0.1f) * VectorUtils.Polar(360f * Random.NextDouble());
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
        }
    }
}
