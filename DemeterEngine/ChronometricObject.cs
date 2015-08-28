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

using System;

#endregion

namespace DemeterEngine
{
    public class ChronometricObject
    {

        public int InitialFrame { get; private set; }

        public double InitialTime { get; private set; }

		/// <summary>
		/// The current number of frames the object has been alive for.
		/// </summary>
        public int LocalFrame { get; private set; }

		/// <summary>
		/// The current number of milliseconds the object has been alive for.
		/// </summary>
        public double LocalTime { get; private set; }

		/// <summary>
		/// Whether or not this object is keeping time.
		/// </summary>
        public bool KeepingTime { get; private set; }

        public ChronometricObject(bool keepTime = false, int initialFrame = 0, double initialTime = 0)
        {
            KeepingTime  = keepTime;
            InitialFrame = initialFrame;
            InitialTime  = initialTime;
            LocalFrame   = initialFrame;
            LocalTime    = initialTime;
        }

        public void UpdateTime()
        {
            if (KeepingTime)
            {
                LocalFrame++;
                LocalTime += GlobalGameTimer.DeltaTime;
            }
        }

        /// <summary>
        /// Reset this object's internal clock.
        /// </summary>
        public void ResetTime()
        {
            LocalFrame = InitialFrame;
            LocalTime = InitialTime;
        }

        /// <summary>
        /// Reset this object's internal clock.
        /// </summary>
        public void ResetTime(int initialFrame, double initialTime)
        {
            LocalFrame = initialFrame;
            LocalTime = initialTime;
        }

        #region Timing Functions

        public bool AtFrame(int frame)
        {
            return LocalFrame == frame;
        }

        public bool AtTime(double time)
        {
            return Math.Abs(LocalTime - time) <= GlobalGameTimer.DeltaTime;
        }

        public bool AfterFrame(int frame)
        {
            return LocalFrame >= frame;
        }

        public bool AfterTime(double time)
        {
            return LocalTime >= time;
        }

        public bool BeforeFrame(int frame)
        {
            return LocalFrame <= frame;
        }

        public bool BeforeTime(double time)
        {
            return LocalTime <= time;
        }

        public bool DuringFrame(int start, int end)
        {
            return start <= LocalFrame && LocalFrame <= end;
        }

        public bool DuringTime(double start, double end)
        {
            return start <= LocalTime && LocalTime <= end;
        }

        public bool OutsideFrame(int start, int end)
        {
            return start >= LocalFrame || LocalFrame >= end;
        }

        public bool OutsideTime(double start, double end)
        {
            return start >= LocalTime || LocalTime >= end;
        }

        public bool AtFrameIntervals(int interval, int start = 0, int? end = null)
        {
            var modded = (LocalFrame - start) % interval;
            return (end.HasValue ? DuringFrame(start, end.Value) : AfterFrame(start)) && modded == 0;
        }

        public bool AtTimeIntervals(double interval, double start = 0, double end = Double.PositiveInfinity)
        {
            var modded = (LocalTime - start) % interval;
            return DuringTime(start, end) &&
                   0 <= modded && modded <= interval;
        }

        public bool DuringFrameIntervals(int interval, int start = 0, int? end = null)
        {
            return (end.HasValue ? DuringFrame(start, end.Value) : AfterFrame(start)) && 
                   (LocalFrame - start) % (2 * interval) < interval;
        }

        public bool DuringTimeIntervals(double interval, double start = 0, double end = Double.PositiveInfinity)
        {
            return DuringTime(start, end) && 
                   (LocalTime - start) % (2 * interval) < interval;
        }

        #endregion

    }
}
