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
 * Date of Creation: 6/29/2015
 * 
 * Description
 * ===========
 * A builtin Collidable subclass representing an axis-oriented ellipse.
 */

#endregion

#region Using Statements

using Microsoft.Xna.Framework;
using DemeterEngine.Maths;
using DemeterEngine.Utils;
using System;
using System.Collections.Generic;

#endregion

namespace DemeterEngine.Collision
{
    public class EllipseCollider : Collidable
    {

        /// <summary>
        /// The EllipseCollider's precedence (3).
        /// </summary>
        public override int Precedence { get { return CollisionPrecedences.ELLIPSE; } }

        /// <summary>
        /// The center of the ellipse.
        /// </summary>
        public Vector2 Center { get { return new Vector2((float)X, (float)Y); } }

        /// <summary>
        /// The area of this ellipse.
        /// </summary>
        public double Area { get { return Math.PI * A * B; } }

        /// <summary>
        /// The private internal cache for the perimeter, as it is a little expensive to calculate.
        /// </summary>
        private PropertyCache<double> perimeterCache = new PropertyCache<double>();

        /// <summary>
        /// The perimeter of this ellipse.
        /// 
        /// This uses an approximate method discovered by Ramanujan. It is much
        /// simpler than the ellipctic integral or infinite series methods, but
        /// has an error term of h^5 where h = ((A - B)/(A + B))^2.
        /// </summary>
        public double Perimeter
        {
            get
            {
                if (perimeterCache.dirty)
                {
                    var h = 3 * Math.Pow((A - B) / (A + B), 2.0);
                    perimeterCache.Clean = Math.PI * (A + B) * (1 + h / (10 + Math.Sqrt(4 - h)));
                }
                return perimeterCache.Value;
            }
        }

        /// <summary>
        /// The x coordinate of the center of the ellipse.
        /// </summary>
        public double X { get; private set; }

        /// <summary>
        /// The y coordinate of the center of the ellipse.
        /// </summary>
        public double Y { get; private set; }

        /// <summary>
        /// The half-length of the horizontal axis of the ellipse.
        /// </summary>
        public double A { get; private set; }

        /// <summary>
        /// The half-length of the vertical axis of the ellipse.
        /// </summary>
        public double B { get; private set; }

        /// <summary>
        /// The ellipse's bounding box.
        /// </summary>
        /// <returns></returns>
        public override double[] BoundingBox()
        {
            return new double[] { X - A, Y - B, 2 * A, 2 * B };
        }

        public EllipseCollider()
            : this(0, 0, 0, 0) { }

        public EllipseCollider(double r)
            : this(0, 0, r, r) { }

        public EllipseCollider(double a, double b)
            : this(0, 0, a, b) { }

        public EllipseCollider(double x, double y, double r)
            : this(x, y, r, r) { }

        public EllipseCollider(double x, double y, double a, double b)
        {
            X = x;
            Y = y;
            A = a;
            B = b;
        }

        public EllipseCollider(Vector2 center, double r)
            : this(center.X, center.Y, r, r) { }

        public EllipseCollider(Vector2 center, double a, double b)
            : this(center.X, center.Y, a, b) { }

        public EllipseCollider(Rectangle rect)
            : this(rect.Center.X, rect.Center.Y, rect.Width / 2.0, rect.Height / 2.0) { }

        public EllipseCollider(double[] args)
        {
            var l = args.Length;
            switch (l)
            {
                case 0:
                    X = 0;
                    Y = 0;
                    A = 0;
                    B = 0;
                    break;
                case 1:
                    X = 0;
                    Y = 0;
                    A = args[0];
                    B = args[0];
                    break;
                case 2:
                    X = 0;
                    Y = 0;
                    A = args[0];
                    B = args[1];
                    break;
                case 3:
                    X = args[0];
                    Y = args[1];
                    A = args[2];
                    B = args[2];
                    break;
                case 4:
                    X = args[0];
                    Y = args[1];
                    A = args[2];
                    B = args[3];
                    break;
                default:
                    throw new ArgumentException(
                        String.Format(
                            "Invalid argument count {0} for type \"EllipseCollider\"", l)
                            );
            }
        }

        /// <summary>
        /// Set the center of the ellipse to the given position.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetPosition(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Set the center of the ellipse to the given position.
        /// </summary>
        /// <param name="pos"></param>
        public void SetPosition(Vector2 pos)
        {
            X = pos.X;
            Y = pos.Y;
        }

        /// <summary>
        /// Translate the ellipse by the given amount.
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        public void Translate(double dx, double dy)
        {
            X += dx;
            Y += dy;
        }

        /// <summary>
        /// Translate the ellipse by the given amount.
        /// </summary>
        /// <param name="delta"></param>
        public void Translate(Vector2 delta)
        {
            X += delta.X;
            Y += delta.Y;
        }

        /// <summary>
        /// Rotate the ellipse about the center by the given amount. 
        /// 
        /// Ellipses are axis-oriented, so attempting to rotate an EllipseCollider whose
        /// vertical and horizontal axis lengths are different simply results in an error.
        /// Otherwise, since a circle rotated about its center results in the same circle,
        /// the method does nothing.
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="degrees"></param>
        public void Rotate(double angle, bool degrees = true)
        {
            if (A != B)
                throw new ArgumentException("Cannot rotate non-circular EllipseCollider.");
            // Literally do nothing, a circle rotated about its center is the same.
        }

        /// <summary>
        /// Rotate the ellipse about a given point by the given amount.
        /// 
        /// Ellipses are axis-oriented, so attempting to rotate an EllipseCollider whose
        /// vertical and horizontal axis lengths are different simply results in an error.
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="origin"></param>
        /// <param name="degrees"></param>
        /// <param name="relative"></param>
        public void Rotate(double angle, Vector2 origin, bool degrees = true, bool relative = true)
        {
            if (A != B)
                throw new ArgumentException("Cannot rotate non-circular EllipseCollider.");
            SetPosition(VectorUtils.Rotate(Center, angle, origin, degrees, relative));
        }

        /// <summary>
        /// Scale the ellipse by the given amount.
        /// </summary>
        /// <param name="amount"></param>
        public void Scale(double amount)
        {
            A *= amount;
            B *= amount;
        }

        /// <summary>
        /// Check if this EllipseCollider is colliding with the given PointCollider.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public CollisionResponse CollidingWith(PointCollider point)
        {
            return new CollisionResponse(this, point,
                Math.Pow((point.X - X) / A, 2.0) + Math.Pow((point.Y - Y) / B, 2.0) <= 1);
        }

        /// <summary>
        /// Check if this EllipseCollider is colliding with the given Vector2.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public CollisionResponse CollidingWith(Vector2 point)
        {
            return CollidingWith(new PointCollider(point));
        }

        /// <summary>
        /// Check if this EllipseCollider is colliding with the given Point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public CollisionResponse CollidingWith(Point point)
        {
            return CollidingWith(new PointCollider(point));
        }

        /// <summary>
        /// Check if this EllipseCollider is colliding with the given SegmentCollider.
        /// </summary>
        /// <param name="segment"></param>
        /// <returns></returns>
        public CollisionResponse CollidingWith(SegmentCollider segment)
        {
            var c_res = new CollisionResponse(this, segment, false);
            double X1 = segment.Start.X,
                   X2 = segment.End.X,
                   Y1 = segment.Start.Y,
                   Y2 = segment.End.Y;
            var intersections = EllipseUtils.EllipseLineIntersections(X, Y, A, B, X1, Y1, X2, Y2);
            if (intersections.Length == 0)
                return c_res;

            var valid = new List<Vector2>();
            foreach (var poi in intersections)
            {
                if (LinearUtils.IsPointInSegmentRange(
                        poi.X, poi.Y, X1, Y1, X2, Y2))
                    valid.Add(poi);
            }

            var count = valid.Count;
            if (count == 0)
                return c_res;

            c_res.SetAttr<int>("NumIntersections", count);
            c_res.SetAttr<Vector2[]>("IntersectionPoints", valid.ToArray());

            return c_res;
        }

        /// <summary>
        /// Check if this EllipseCollider is colliding with the given RectCollider.
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public CollisionResponse CollidingWith(RectCollider rect)
        {
            bool result = false;

            // Check if this works first.
            /*
            if (A == B)
            {
                double rx = rect.X + rect.W / 2, ry = rect.Y + rect.H / 2;
                double x_offset = Math.Abs(X - rx);
                double y_offset = Math.Abs(Y - ry);
                double half_width = rect.W / 2;
                double half_height = rect.H / 2;

                if (x_offset > (half_width + A))
                    result = false;
                else if (y_offset > (half_height + A))
                    result = false;
                else 
                {
                    if (x_offset <= half_width)
                        result = true;
                    else if (y_offset <= half_height)
                        result = true;
                    else
                    {
                        double deltax = x_offset - half_width;
                        double deltay = y_offset - half_height;
                        double dist = Math.Pow(deltax, 2) + Math.Pow(deltay, 2);
                        result = dist <= A * A ? true : false;
                    }
                }
            }
            else
            {
             */
            // For the math, see:
            //   http://www.geometrictools.com/Documentation/IntersectionRectangleEllipse.pdf

            // NOTE: Look into using the minkowski sum algorithm suggested there instead,
            // it looks substantially faster.
            var c = Center;
            var corners = rect.Coords;
            for (int i = 0; i < 4; i++)
            {
                if (EllipseUtils.EllipseOverlapSegment(c, A, B, corners[(i - 1) % 4], corners[i]))
                {
                    result = true;
                    break;
                }
            }
            result = rect.X <= X && X <= rect.X + rect.W &&
                     rect.Y <= Y && Y <= rect.Y + rect.H;
            /*}*/
            return new CollisionResponse(this, rect, result);
        }

        /// <summary>
        /// Check if this EllipseCollider is colliding with the given EllipseCollider.
        /// </summary>
        /// <param name="ellipse"></param>
        /// <returns></returns>
        public CollisionResponse CollidingWith(EllipseCollider ellipse)
        {
            if (A == B && ellipse.A == ellipse.B)
                return new CollisionResponse(this, ellipse,
                    Vector2.Distance(Center, ellipse.Center) <= A + ellipse.A
                    );
            throw new NotImplementedException(
                "No algorithm for ellipse-to-ellipse collision detection exists yet, sorry!");
        }

    }
}
