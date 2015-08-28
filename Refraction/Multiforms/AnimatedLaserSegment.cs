using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Refraction_V2.Multiforms
{
    public class AnimatedLaserSegment : LaserSegment
    {

        /// <summary>
        /// The scale at which to draw this laser segment.
        /// </summary>
        public float Scale { get; private set; }

        // Animators are functions that take the laser's LocalFrame as input and
        // output a value which is used to update an existing property. For example,
        // the radius animator animates the laser's radius property.
        #region Animators

        /// <summary>
        /// The animating function for the laser's scale.
        /// </summary>
        public Func<int, float> ScaleAnimator { get; private set; }

        /// <summary>
        /// Whether or not the scale animator is an additive function of scale or an in-place one.
        /// </summary>
        private bool scaleAnimatorIsAdditive = false;

        /// <summary>
        /// The animating function for the laser's start position.
        /// </summary>
        public Func<int, Vector2> StartPosAnimator { get; private set; }

        /// <summary>
        /// Whether or not the start pos animator is an additive function of the start pos or an in-place one.
        /// </summary>
        private bool startPosAnimatorIsAdditive = false;

        /// <summary>
        /// The animating function for the laser's end position.
        /// </summary>
        public Func<int, Vector2> EndPosAnimator { get; private set; }

        /// <summary>
        /// Whether or not the end pos animator is an additive function of the end pos or an in-place one.
        /// </summary>
        private bool endPosAnimatorIsAdditive = false;

        #endregion

        public AnimatedLaserSegment(Vector2 start, Vector2 end, Color color, float scale = 1f)
            : base(start, end, color, true)
        {
            Scale = scale;
        }

        protected T UpdateAnimator<T>(Func<int, T> animator, bool additive, T val)
        {
            if (animator == null)
                return val;
            var newVal = animator(LocalFrame);

            if (additive)
            {
                return (dynamic)newVal + (dynamic)val;
            }
            else
            {
                return newVal;
            }
        }

        /// <summary>
        /// Assign the laser a scale animator function.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="additive"></param>
        public void SetScaleAnimator(Func<int, float> func, bool additive = true)
        {
            ScaleAnimator = func;
            scaleAnimatorIsAdditive = additive;
        }

        /// <summary>
        /// Assign the laser a start pos animator function.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="additive"></param>
        public void SetStartPosAnimator(Func<int, Vector2> func, bool additive = true)
        {
            StartPosAnimator = func;
            startPosAnimatorIsAdditive = additive;
        }

        /// <summary>
        /// Assign the laser an end pos animator function.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="additive"></param>
        public void SetEndPosAnimator(Func<int, Vector2> func, bool additive = true)
        {
            EndPosAnimator = func;
            endPosAnimatorIsAdditive = additive;
        }

        public virtual void Update()
        {
            UpdateTime();

            Scale = UpdateAnimator<float>(ScaleAnimator, scaleAnimatorIsAdditive, Scale);
            Start = UpdateAnimator<Vector2>(StartPosAnimator, startPosAnimatorIsAdditive, Start);
            End   = UpdateAnimator<Vector2>(EndPosAnimator, endPosAnimatorIsAdditive, End);
        }

        public void Render()
        {
            Render(Scale);
        }

    }
}
