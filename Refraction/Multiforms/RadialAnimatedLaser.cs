using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DemeterEngine;
using DemeterEngine.Maths;

namespace Refraction_V2.Multiforms
{
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
