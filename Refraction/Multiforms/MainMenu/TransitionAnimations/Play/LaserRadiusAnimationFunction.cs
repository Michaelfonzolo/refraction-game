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

namespace Refraction_V2.Multiforms.MainMenu.TransitionAnimations.Play
{

    /// <summary>
    /// A special piecewise function that defines the radii of the radial lasers
    /// in the PlayAnimationForm. It's a C1 curve constructed from a parabola,
    /// attached to a linear function, attached to another parabola.
    /// </summary>
    public class LaserRadiusAnimationFunction
    {

        // Default shape parameters.
        public const float DEFAULT_Y0 = 0f;
        public const float DEFAULT_Y1 = 250f;
        public const float DEFAULT_X0 = 0f;
        public const float DEFAULT_X1 = 30f;
        public const float DEFAULT_X2 = 32f;
        public const float DEFAULT_X3 = 120f;
        public const float DEFAULT_X4 = 120.5f;

        // Shape parameters.
        public float Y0 { get; private set; }
        public float Y1 { get; private set; }
        public float X0 { get; private set; }
        public float X1 { get; private set; }
        public float X2 { get; private set; }
        public float X3 { get; private set; }
        public float X4 { get; private set; }

        // Auxiliary constants
        private float M1, M2;

        public LaserRadiusAnimationFunction(
            float y0 = DEFAULT_Y0, float y1 = DEFAULT_Y1, float x0 = DEFAULT_X0,
            float x1 = DEFAULT_X1, float x2 = DEFAULT_X2, float x3 = DEFAULT_X3, float x4 = DEFAULT_X4)
        {
            Y0 = y0;
            Y1 = y1;
            X0 = x0;
            X1 = x1;
            X2 = x2;
            X3 = x3;
            X4 = x4;

            CheckRestrictions();

            M1 = (Y0 - Y1) / (float)Math.Pow(X0 - X1, 2);
            M2 = (X2 - X1) * M1;
        }

        private void CheckRestrictions()
        {
            if (!(X0 < X1 && X1 < X2 && X2 < X3 && X3 < X4))
            {
                throw new ArgumentException("Invalid laser radius animation arguments.");
            }
        }

        public float Func(int x)
        {
            if (X0 <= x && x <= X2)
            {
                return Y1 + M1 * (float)Math.Pow(x - X1, 2);
            }
            else if (X2 <= x && x <= X3)
            {
                return Y1 + M2 * (2 * x - X1 - X2);
            }
            return Y1 + M2 * (X3 - X1 - X2 + X4 + (float)Math.Pow(x - X4, 2) / (X3 - X4));
        }

    }
}
