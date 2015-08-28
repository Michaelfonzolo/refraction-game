using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DemeterEngine;
using DemeterEngine.Multiforms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Refraction_V2.Multiforms.ForegroundContent
{
    public class ClickParticle : ChronometricObject
    {

        public Vector2 Position { get; private set; }

        public Vector2 Velocity { get; private set; }

        public int LifeTime { get; private set; }

        public Color Colour { get; private set; }

        public bool Dead { get { return LocalFrame > LifeTime; } }

        public ClickParticle(Vector2 pos, Vector2 velocity, int lifeTime, Color colour) : base(true)
        {
            Position = pos;
            Velocity = velocity;
            LifeTime = lifeTime;
            Colour = colour;
        }

        public void Update()
        {
            UpdateTime();
            Position += (1.5f - LocalFrame / (float)LifeTime) * Velocity;
        }

        public void Render()
        {
            var texture = Assets.Shared.Images.LaserCap;
            var offset = new Vector2(texture.Width, texture.Height / 2f);

            var alpha = 1f - LocalFrame / (float)LifeTime;
            var col1 = new Color(Colour, alpha);
            var col2 = new Color(Color.White, alpha);

            DisplayManager.Draw(texture, Position, null, col1, 0f, offset, 0.3f, SpriteEffects.None, 0f);
            DisplayManager.Draw(texture, Position, null, col1, (float)Math.PI, offset, 0.3f, SpriteEffects.None, 0f);

            DisplayManager.Draw(texture, Position, null, col2, 0f, offset, 0.2f, SpriteEffects.None, 0f);
            DisplayManager.Draw(texture, Position, null, col2, (float)Math.PI, offset, 0.2f, SpriteEffects.None, 0f);
        }
    }
}
