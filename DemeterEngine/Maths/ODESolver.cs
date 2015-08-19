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

namespace DemeterEngine.Maths
{
    public class ODESolver
    {

		/// <summary>
		/// The function F in the differential equation we're solving:
		/// y' = F(x, y).
		/// </summary>
        public Func<double, double, double> Function { get; private set; }

		/// <summary>
		/// The amount by which to increment x.
		/// </summary>
        public double stepSize { get; private set; }

		/// <summary>
		/// The initial x value of the equation solver.
		/// </summary>
        public double initialX { get; private set; }

		/// <summary>
		/// The initial y value of the equation solver.
		/// </summary>
        public double initialY { get; private set; }

		/// <summary>
		/// The current x value of the equation solver.
		/// </summary>
        public double currentX { get; private set; }

		/// <summary>
		/// The current y value approximation.
		/// </summary>
        public double currentY { get; private set; }

        public ODESolver(Func<double, double, double> f, double stepSize, double initialX, double initialY)
        {
            Function = f;
            this.stepSize = stepSize;
            this.initialX = initialX;
            this.initialY = initialY;
            this.currentX = initialX;
            this.currentY = initialY;
        }

        public double GetNext()
        {
            double k1, k2, k3, k4;
            double h_2 = stepSize / 2;
            currentX += stepSize;

            k1 = Function(currentX,            currentY                );
            k2 = Function(currentX + h_2,      currentY + h_2 * k1     );
            k3 = Function(currentX + h_2,      currentY + h_2 * k2     );
            k4 = Function(currentX + stepSize, currentY + stepSize * k3);

            currentY += stepSize / 6 * (k1 + 2 * (k2 + k3) + k4);
            return currentY;
        }
    }
}
