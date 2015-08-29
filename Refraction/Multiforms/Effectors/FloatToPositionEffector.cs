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

using System;

using DemeterEngine.Effectors;

using Microsoft.Xna.Framework;

using Refraction_V2.Utils;

#endregion

namespace Refraction_V2.Multiforms.Effectors
{

    /// <summary>
    /// An effector which causes it's associated ITransitionalForm to float to
    /// a given end position over a certain duration.
    /// </summary>
    public class FloatToPositionEffector : Effector
    {

        /// <summary>
        /// The start position of the form.
        /// </summary>
        public Vector2 StartPosition { get; private set; }

        /// <summary>
        /// The end position of the form.
        /// </summary>
        public Vector2 EndPosition { get; private set; }

        /// <summary>
        /// The duration of the effector.
        /// </summary>
        public int Duration { get; private set; }

        /// <summary>
        /// A parameter which controls how quickly the form floats to the new position.
        /// </summary>
        public double Tension { get; private set; }

        private Vector2 _delta; // = EndPosition - StartPosition

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

        private float TimeFunction(int n)
        {
            return 1f - (float)Math.Pow(Math.Abs(n / (float)Duration - 1f), Tension);
        }

        public override void Update()
        {
            base.Update();

            // Get the new position and update the form. The variable "time" here refers
            // to the variable t in the equation P = S + t*(E - S). It doesn't actually
            // represent the local time of the effector.
            var time = TimeFunction(LocalFrame);
            var nextPosition = StartPosition + _delta * time;
            ((ITransitionalForm)Form).SetPosition(nextPosition, PositionType.Center);

            if (LocalFrame >= Duration)
            {
                Kill();
            }
        }

    }
}
