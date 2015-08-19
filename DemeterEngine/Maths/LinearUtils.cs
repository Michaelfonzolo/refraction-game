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
 * A set of geometric utility methods regarding lines.
 */

#endregion

#region Using Statements

using Microsoft.Xna.Framework;

#endregion

namespace DemeterEngine.Maths
{
    public static class LinearUtils
    {

        /// <summary>
        /// Check if a point is in the "range" of a segment.
        /// 
        /// This essentially checks if the point is within the segment's bounding box.
        /// </summary>
        public static bool IsPointInSegmentRange(
            double px, double py,
            double x1, double y1,
            double x2, double y2)
        {
            if (x1 == x2)
                return (y1 <= py) == (py <= y2);
            return (x1 <= px) == (px <= x2);
        }

        /// <summary>
        /// Calculate the intersection point between two lines (or null if they are parallel).
        /// </summary>
        public static Vector2? LineIntersectionPoint(
            double x1, double y1, double x2, double y2,
            double x3, double y3, double x4, double y4)
        {
            if (((x1 == x2) && (x3 == x4)) || ((y1 == y2) && (y3 == y4)))
                return null;
            else if (x1 == x2)
            {
                double m1 = (y4 - y3) / (x4 - x3);
                double b1 = y3 - m1 * x3;
                double py = m1 * x1 + b1;
                return new Vector2((float)x1, (float)py);
            }
            else if (x3 == x4)
            {
                double m1 = (y2 - y1) / (x2 - x1);
                double b1 = y1 - m1 * x1;
                double py = m1 * x3 + b1;
                return new Vector2((float)x3, (float)py);
            }
            else
            {
                double m1 = (y2 - y1) / (x2 - x1);
                double m2 = (y4 - y3) / (x4 - x3);

                if (m1 == m2)
                    return null;

                double b1 = y1 - m1 * x1;
                double b2 = y3 - m2 * x3;

                double px = (b2 - b1) / (m1 - m2);
                double py = m1 * px + b1;

                return new Vector2((float)px, (float)py);
            }
        }

        /// <summary>
        /// Calculate the intersection point between a line and a line segment (or null if they
        /// do not intersect).
        /// </summary>
        public static Vector2? LineToSegmentIntersectionPoint(
            double x1, double y1, double x2, double y2,
            double x3, double y3, double x4, double y4)
        {
            var p = LineIntersectionPoint(x1, y1, x2, y2, x3, y3, x4, y4);
            if (p.HasValue && IsPointInSegmentRange(p.Value.X, p.Value.Y, x3, y3, x4, y4))
                return p;
            return null;
        }

        /// <summary>
        /// Calculate the intersection point between two line segments (or null if they
        /// do not intersect).
        /// </summary>
        public static Vector2? SegmentIntersectionPoint(
            double x1, double y1, double x2, double y2,
            double x3, double y3, double x4, double y4)
        {
            var p = LineIntersectionPoint(x1, y1, x2, y2, x3, y3, x4, y4);
            if (!p.HasValue)
                return null;
            var v = p.Value;
            if (IsPointInSegmentRange(v.X, v.Y, x1, y1, x2, y2) &&
                IsPointInSegmentRange(v.X, v.Y, x3, y3, x4, y4))
                return p;
            return null;
        }

        /// <summary>
        /// Check if a point lies on a given line.
        /// </summary>
        public static bool PointOnLine(
            double px, double py,
            double x1, double y1, double x2, double y2)
        {
            if (x1 == x2)
                return px == x1 && ((y1 <= py) == (py <= y2));
            else if (y1 == y2)
                return py == y1 && ((x1 <= px) == (px <= y1));
            double m = (y2 - y1) / (x2 - x1);
            double b = y1 - m * x1;
            return py == m * px + b;
        }

        /// <summary>
        /// Check if a point lies on a given line.
        /// </summary>
        public static bool PointOnSegment(
            double px, double py,
            double x1, double y1, double x2, double y2)
        {
            return PointOnLine(px, py, x1, y1, x2, y2) && IsPointInSegmentRange(px, py, x1, y1, x2, y2);
        }

    }
}
