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
 * Date of Creation: 6/25/2015
 * 
 * Description
 * ===========
 * A set of geometric utility methods regarding rectangles.
 */

#endregion


namespace DemeterEngine.Maths
{
    public static class RectUtils
    {

        /// <summary>
        /// Check if two rectangles are overlapping.
        /// </summary>
        public static bool Collision(
            double x1, double y1, double w1, double h1,
            double x2, double y2, double w2, double h2)
        {
            return x1 <= x2 + w2 &&
                   x1 + w1 >= x2 &&
                   y1 <= y2 + h2 &&
                   y1 + h1 >= y2;
        }

        /// <summary>
        /// Check if the first rectangle entirely encloses the second rectangle.
        /// </summary>
        public static bool Contains(
            double x1, double y1, double w1, double h1,
            double x2, double y2, double w2, double h2)
        {
            return x1 <= x2 && x1 + w1 >= x2 + w2 &&
                   y1 <= y2 && y1 + h1 >= y2 + h2;
        }

        /// <summary>
        /// Check if the two given rectangles form a cross pattern.
        /// </summary>
        public static bool Crosses(
            double x1, double y1, double w1, double h1,
            double x2, double y2, double w2, double h2)
        {
            return (
                    x1 <= x2 && x1 + w1 >= x2 + w2 &&
                    y1 >= y2 && y1 + h1 <= y2 + h2
                ) || (
                    x1 >= x2 && x1 + w1 <= x2 + w2 &&
                    y1 <= y2 && y1 + h1 >= y2 + h2
                );
        }

    }
}
