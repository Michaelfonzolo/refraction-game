using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Refraction_V2.Multiforms
{
    public class AnimatedLaserSegment : LaserSegment
    {

        public float Scale { get; private set; }

        public AnimatedLaserSegment(Vector2 start, Vector2 end, Color color, float scale = 1f)
            : base(start, end, color, true)
        {
            Scale = scale;
        }

        public virtual void Update()
        {
            UpdateTime();
        }

        public void Render()
        {
            Render(Scale);
        }

    }
}
