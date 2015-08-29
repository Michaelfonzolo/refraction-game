// !!! INCOMPLETE !!!

using DemeterEngine.Multiforms;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Refraction_V2.Multiforms.MainMenu.TransitionAnimations.Options
{
    public class OptionsAnimationForm : Form
    {

        public static readonly Func<int, float> WhiteOutAlphaFunction = n => 0/*(float)Math.Pow(n / 200f, 7)*/;

        public Vector2 Center { get; private set; }

        public List<AnimatedLaserSegment> Lasers = new List<AnimatedLaserSegment>();


        public OptionsAnimationForm(Vector2 center)
            : base(true)
        {
            Center = center;
        }

        public override void Update()
        {
            base.Update();

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
        }

    }
}
