using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DemeterEngine.Effectors;
using DemeterEngine.Multiforms;
using Microsoft.Xna.Framework;
using Refraction_V2.Utils;

namespace Refraction_V2.Multiforms.Effectors
{
    public class FloatToPositionEffector : Effector
    {

        public Vector2 StartPosition { get; private set; }

        public Vector2 EndPosition { get; private set; }

        public int Duration { get; private set; }

        public double Tension { get; private set; }

        private Vector2 _delta;

        public FloatToPositionEffector(Vector2 finalCenter, int duration, double tension = 2.2)
            : base()
        {
            EndPosition = finalCenter;
            Duration = duration;
            Tension = tension;
        }

        protected override void Initialize()
        {
            base.Initialize();
            StartPosition = ((ITransitionalForm)Form).GetPosition(PositionType.Center);
            _delta = EndPosition - StartPosition;
        }

        public override void Update()
        {
            base.Update();

            var time = 1f - (float)Math.Pow(Math.Abs(LocalFrame / (float)Duration - 1f), Tension);
            var nextPosition = StartPosition + _delta * time;
            ((ITransitionalForm)Form).SetPosition(nextPosition, PositionType.Center);

            if (LocalFrame >= Duration)
            {
                Kill();
            }
        }

    }
}
