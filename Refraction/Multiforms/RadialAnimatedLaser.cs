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

using DemeterEngine.Maths;

using Microsoft.Xna.Framework;

using System;

#endregion

namespace Refraction_V2.Multiforms
{

    /// <summary>
    /// An AnimatedLaserSegment defined by a center, a radius, and an angle, rather
    /// than just a start and an end position.
    /// </summary>
    public class RadialAnimatedLaser : AnimatedLaserSegment
    {

        private float _angle;

        /// <summary>
        /// The angle the laser is aimed in.
        /// </summary>
        public float Angle
        {
            get { return _angle; }
            set
            {
                _angle = value;
                ResetEndPosition();
            }
        }

        private float _radius;

        /// <summary>
        /// The radius of the laser.
        /// </summary>
        public float Radius
        {
            get { return _radius; }
            set
            {
                _radius = value;
                ResetEndPosition();
            }
        }

        private void ResetEndPosition()
        {
            End = Start + Radius * VectorUtils.Polar(Angle, Degrees);
        }

        /// <summary>
        /// Whether or not the angles are measured in degrees or radians.
        /// </summary>
        public bool Degrees { get; private set; }

        /// <summary>
        /// The center of the laser from which it emanates.
        /// </summary>
        public Vector2 Center { get { return Start; } }

        // Animators are functions that take the laser's LocalFrame as input and
        // output a value which is used to update an existing property. For example,
        // the radius animator animates the laser's radius property.
        #region Animator Related Properties

        /// <summary>
        /// An animating function for the laser's radius.
        /// </summary>
        public Func<int, float> RadiusAnimator { get; private set; }

        /// <summary>
        /// Whether or not the radius animator is an additive function or an in place one.
        /// </summary>
        private bool radiusAnimatorIsAdditive = false;

        /// <summary>
        /// An animating function for the laser's angle.
        /// </summary>
        public Func<int, float> AngleAnimator { get; private set; }

        /// <summary>
        /// Whether or not the angle animator is an additive function or an in place one.
        /// </summary>
        private bool angleAnimatorIsAdditive = false;

        #endregion

        public RadialAnimatedLaser(
            Vector2 center, float angle, float radius, Color color, float scale = 1f, bool degrees = true)
            : base(center, center + radius * VectorUtils.Polar(angle, degrees), color, scale)
        {
            Degrees = degrees;
            Radius = radius;
            Angle = angle;

            RadiusAnimator = null;
            AngleAnimator = null;
        }

        /// <summary>
        /// Assign the laser a radius animator function.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="additive"></param>
        public void SetRadiusAnimator(Func<int, float> func, bool additive = true)
        {
            RadiusAnimator = func;
            radiusAnimatorIsAdditive = additive;
        }

        /// <summary>
        /// Assign the laser an angle animator function.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="additive"></param>
        public void SetAngleAnimator(Func<int, float> func, bool additive = true)
        {
            AngleAnimator = func;
            angleAnimatorIsAdditive = additive;
        }

        public override void Update()
        {
            base.Update();

            Radius = UpdateAnimator<float>(RadiusAnimator, radiusAnimatorIsAdditive, Radius);
            Angle  = UpdateAnimator<float>(AngleAnimator, angleAnimatorIsAdditive, Angle);
        }
    }
}
