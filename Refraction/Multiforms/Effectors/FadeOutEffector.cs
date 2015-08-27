using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DemeterEngine.Effectors;

namespace Refraction_V2.Multiforms.Effectors
{
    public class FadeOutEffector : Effector
    {

        public int Duration { get; private set; }

        public FadeOutEffector(int frameDuration)
            : base()
        {
            Duration = frameDuration;
        }

        private float AlphaFunction(int frame)
        {
            return 1 - frame / (float)Duration;
        }

        public override void Update()
        {
            base.Update();

            var form = (ITransitionalForm)Form;
            form.SetAlpha(255 * AlphaFunction(LocalFrame));

            if (LocalFrame >= Duration)
            {
                form.SetAlpha(0);
                Kill();
            }
        }

    }
}
