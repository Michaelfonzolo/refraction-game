﻿#region License

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

namespace DemeterEngine.Maths
{
    public static class VectorUtils
    {

        public static Vector2 FromPoint(Point p)
        {
            return new Vector2(p.X, p.Y);
        }

        public static Vector2 FromPoint(System.Drawing.Point p)
        {
            return new Vector2(p.X, p.Y);
        }

        /// <summary>
        /// The unit vector pointing left (<-1, 0>).
        /// </summary>
        public static Vector2 Left = new Vector2(-1, 0);

        /// <summary>
        /// The unit vector pointing right (<1, 0>).
        /// </summary>
        public static Vector2 Right = new Vector2(1, 0);

        /// <summary>
        /// The unit vector pointing up (<0, 1>).
        /// </summary>
        public static Vector2 Up = new Vector2(0, 1);

        /// <summary>
        /// The unit vector point down (<0, -1>).
        /// </summary>
        public static Vector2 Down = new Vector2(0, -1);

        /// <summary>
        /// The up direction relative to the game world. Since the game coordinate space is
        /// flipped on it's y-axis, this corresponds to the vector <0, -1>.
        /// </summary>
        public static Vector2 GameWorldUp = Down;

        /// <summary>
        /// The down direction relative to the game world. Since the game coordinate space is
        /// flipped on it's y-axis, this corresponds to the vector <0, 1>.
        /// </summary>
        public static Vector2 GameWorldDown = Up;

        public static Vector2 Ones = new Vector2(1, 1);

        /// <summary>
        /// Return the unit vector pointing at the given angle relative to the positive x-axis.
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static Vector2 Polar(double angle, bool degrees = true)
        {
            if (degrees)
                angle *= MathConstants.DEG_TO_RAD;
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        /// <summary>
        /// Return the unit vector pointing at the given angle relative to the positive x-axis,
        /// centered at the given origin.
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="origin"></param>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static Vector2 Polar(double angle, Vector2 origin, bool degrees = true)
        {
            if (degrees)
                angle *= MathConstants.DEG_TO_RAD;
            return new Vector2((float)Math.Cos(angle) + origin.X, (float)Math.Sin(angle) + origin.Y);
        }

        /// <summary>
        /// Rotate a vector by the given amount relative to the given point, and return the result.
        /// </summary>
        public static Vector2 Rotate(
            Vector2 vec, double angle, Vector2 origin,
            bool degrees = true, bool absoluteOrigin = true)
        {
            if (degrees)
                angle *= MathConstants.DEG_TO_RAD;

            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            if (absoluteOrigin)
            {
                double x = vec.X - origin.X;
                double y = vec.Y - origin.Y;

                double nx = x * cos - y * sin + origin.X;
                double ny = x * sin + y * cos + origin.Y;

                return new Vector2((float)nx, (float)ny);
            }
            else
            {
                double x = origin.X;
                double y = origin.Y;

                double nx = (1 - cos) * x + sin * y + vec.X;
                double ny = (1 - cos) * y - sin * x + vec.Y;

                return new Vector2((float)nx, (float)ny);
            }
        }

        /// <summary>
        /// Return the normalization of a given vector.
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static Vector2 ToNormalized(Vector2 vec)
        {
            return vec / vec.Length();
        }

        /// <summary>
        /// Scale a vector by the given amount relative to the given point, and return the result.
        /// </summary>
        /// <param name="vec"></param>
        /// <param name="amount"></param>
        /// <param name="origin"></param>
        /// <param name="absoluteOrigin"></param>
        /// <returns></returns>
        public static Vector2 Scale(
            Vector2 vec, double amount, Vector2 origin, bool absoluteOrigin = true)
        {
            if (!absoluteOrigin)
                return vec + origin * (1f - (float)amount);
            return (vec - origin) * (float)amount + origin;
        }

    }
}
