using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DemeterEngine.Maths;
using DemeterEngine.Multiforms;
using Microsoft.Xna.Framework;

namespace Refraction_V2.Multiforms.MainMenu.TransitionAnimations.Credits
{
    public class CreditsAnimationForm : Form
    {

        public Vector2 Center { get; private set; }

        public List<AnimatedLaserSegment> Lasers = new List<AnimatedLaserSegment>();

        private Vector2 LastPos;

        public CreditsAnimationForm(Vector2 center)
            : base(true)
        {
            Center = center;
            LastPos = Center;
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
