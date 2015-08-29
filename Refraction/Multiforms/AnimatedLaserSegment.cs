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

using Microsoft.Xna.Framework;

using System;

#endregion

namespace Refraction_V2.Multiforms
{

    /// <summary>
    /// A laser segment that can be animated using animator functions.
    /// </summary>
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
