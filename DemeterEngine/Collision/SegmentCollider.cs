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
 * Date of Creation: 6/28/2015
 * 
 * Description
 * ===========
 * A builtin Collidable subclass representing a line segment.
 */

#endregion

#region Using Statements

using Microsoft.Xna.Framework;
using DemeterEngine.Utils;
using DemeterEngine.Maths;
using System;

#endregion

namespace DemeterEngine.Collision
{
    public class SegmentCollider : Collidable
    {

        /// <summary>
        /// The SegmentCollider's precedence (2).
        /// </summary>
        public override int Precedence { get { return CollisionPrecedences.SEGMENT; } }

        /// <summary>
        /// The center of this line segment.
        /// </summary>
        public Vector2 Center { get { return (Start + End) / 2.0f; } }

        /// <summary>
        /// The area of this line segment (0).
        /// </summary>
        public double Area { get { return 0; } }

        /// <summary>
        /// The perimeter of this line segment (it's length).
        /// </summary>
        public double Perimeter { get { return Length; } }

        // The private internal cache for the length of this line segment.
        private PropertyCache<double> lengthCache = new PropertyCache<double>();

        /// <summary>
        /// The length of this line segment (equivalent to Magnitude).
        /// </summary>
        public double Length
        {
            get
            {
                if (lengthCache.dirty)
                    lengthCache.Clean = Vector2.Distance(Start, End);
                return lengthCache.Value;
            }
        }

        /// <summary>
        /// The magnitude of this line segment (equivalent to Length).
        /// </summary>
        public double Magnitude { get { return Length; } }

        /// <summary>
        /// The squared length of this line segment (equivalent to MagnitudeSquared).
        /// </summary>
        public double LengthSquared { get { return Math.Pow(Length, 2d); } }

        /// <summary>
        /// The squared magnitude of this line segment (equivalent to LengthSquared).
        /// </summary>
        public double MagnitudeSquared { get { return LengthSquared; } }

        /// <summary>
        /// The start coordinate of this line segment.
        /// </summary>
        public Vector2 Start { get; private set; }

        /// <summary>
        /// The end coordinate of this line segment.
        /// </summary>
        public Vector2 End { get; private set; }

        /// <summary>
        /// The bounding box of this line segment.
        /// </summary>
        /// <returns></returns>
        public override double[] BoundingBox()
        {
            double X = Math.Min(Start.X, End.X);
            double Y = Math.Min(Start.Y, End.Y);
            double W = Math.Max(Start.X, End.X) - X;
            double H = Math.Max(Start.Y, End.Y) - Y;
            return new double[] { X, Y, W, H };
        }

        public SegmentCollider()
            : this(0, 0, 0, 0) { }

        public SegmentCollider(double end_x, double end_y)
            : this(0, 0, end_x, end_y) { }

        public SegmentCollider(double start_x, double start_y, double end_x, double end_y)
        {
            Start = new Vector2((float)start_x, (float)start_y);
            End = new Vector2((float)end_x, (float)end_y);
        }

        public SegmentCollider(Vector2 end)
        {
            Start = Vector2.Zero;
            End = end;
        }

        public SegmentCollider(Vector2 start, Vector2 end)
        {
            Start = start;
            End = end;
        }

        public SegmentCollider(SegmentCollider other)
        {
            Start = other.Start;
            End = other.End;
        }

        public SegmentCollider(double[] args)
        {
            var l = args.Length;
            float x1, y1, x2, y2;
            switch (l)
            {
                case 0:
                    x1 = 0;
                    y1 = 0;
                    x2 = 0;
                    y2 = 0;
                    break;
                case 2:
                    x1 = 0;
                    y1 = 0;
                    x2 = (float)args[0];
                    y2 = (float)args[1];
                    break;
                case 4:
                    x1 = (float)args[0];
                    y1 = (float)args[1];
                    x2 = (float)args[2];
                    y2 = (float)args[3];
                    break;
                default:
                    throw new ArgumentException(
                        String.Format(
                            "Invalid argument count {0} for type \"SegmentCollider\"", l)
                            );
            }
            Start = new Vector2(x1, y1);
            End = new Vector2(x2, y2);
        }

        /// <summary>
        /// Set the start position of this line segment separate from the end position.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetStartPosition(double x, double y)
        {
            Start = new Vector2((float)x, (float)y);
            lengthCache.dirty = true;
        }

        /// <summary>
        /// Set the start position of this line segment separate from the end position.
        /// </summary>
        /// <param name="vec"></param>
        public void SetStartPosition(Vector2 vec)
        {
            Start = vec;
            lengthCache.dirty = true;
        }

        /// <summary>
        /// Set the end position of this line segment separate from the start position.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetEndPosition(double x, double y)
        {
            End = new Vector2((float)x, (float)y);
            lengthCache.dirty = true;
        }

        /// <summary>
        /// Set the end position of this line segment separate from the start position.
        /// </summary>
        /// <param name="vec"></param>
        public void SetEndPosition(Vector2 vec)
        {
            End = vec;
            lengthCache.dirty = true;
        }

        /// <summary>
        /// Translate the start of this line segment separate from the end position.
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        public void TranslateStart(double dx, double dy)
        {
            Start += new Vector2((float)dx, (float)dy);
            lengthCache.dirty = true;
        }

        /// <summary>
        /// Translate the start of this line segment separate from the end position.
        /// </summary>
        /// <param name="delta"></param>
        public void TranslateStart(Vector2 delta)
        {
            Start += delta;
            lengthCache.dirty = true;
        }

        /// <summary>
        /// Translate the end of this line segment separate from the start position.
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        public void TranslateEnd(double dx, double dy)
        {
            End += new Vector2((float)dx, (float)dy);
            lengthCache.dirty = true;
        }

        /// <summary>
        /// Translate the end of this line segment separate from the start position.
        /// </summary>
        /// <param name="delta"></param>
        public void TranslateEnd(Vector2 delta)
        {
            End += delta;
            lengthCache.dirty = true;
        }

        /// <summary>
        /// Translate this line segment by the given amount.
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        public void Translate(double dx, double dy)
        {
            var delta = new Vector2((float)dx, (float)dy);
            Start += delta;
            End += delta;
        }

        /// <summary>
        /// Translate this line segment by the given amount.
        /// </summary>
        /// <param name="delta"></param>
        public void Translate(Vector2 delta)
        {
            Start += delta;
            End += delta;
        }

        /// <summary>
        /// Rotate this line segment by the given amount around its center.
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="degrees"></param>
        public void Rotate(double angle, bool degrees = true)
        {
            Rotate(angle, Center, degrees, true);
        }

        /// <summary>
        /// Rotate this line segment by the given amount relative to the given point.
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="origin"></param>
        /// <param name="degrees"></param>
        /// <param name="relative"></param>
        public void Rotate(double angle, Vector2 origin, bool degrees = true, bool absoluteOrigin = true)
        {
            Start = VectorUtils.Rotate(Start, angle, origin, degrees, absoluteOrigin);
            End = VectorUtils.Rotate(End, angle, origin, degrees, absoluteOrigin);
        }

        /// <summary>
        /// Scale this line segment by the given amount relative to its center.
        /// </summary>
        /// <param name="amount"></param>
        public void Scale(double amount)
        {
            var c = Center;
            Start = VectorUtils.Scale(Start, amount, c, true);
            End = VectorUtils.Scale(End, amount, c, true);
        }

        /// <summary>
        /// Scale this line segment by the given amount relative to the given point.
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="origin"></param>
        /// <param name="relative"></param>
        public void Scale(double amount, Vector2 origin, bool absoluteOrigin = true)
        {
            Start = VectorUtils.Scale(Start, amount, origin, absoluteOrigin);
            End = VectorUtils.Scale(End, amount, origin, absoluteOrigin);
            lengthCache.dirty = true;
        }

        /// <summary>
        /// Check if this SegmentCollider is colliding with the given PointCollider.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public CollisionResponse CollidingWith(PointCollider point)
        {
            var result = LinearUtils.PointOnLine(point.X, point.Y, Start.X, Start.Y, End.X, End.Y);
            return new CollisionResponse(this, point, result);
        }

        /// <summary>
        /// Check if this SegmentCollider is colliding with the given Vector2.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public CollisionResponse CollidingWith(Vector2 point)
        {
            return CollidingWith(new PointCollider(point));
        }

        /// <summary>
        /// Check if this SegmentCollider is colliding with the given Point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public CollisionResponse CollidingWith(Point point)
        {
            return CollidingWith(new PointCollider(point));
        }

        /// <summary>
        /// Check if this SegmentCollider is colliding with the given SegmentCollider.
        /// </summary>
        /// <param name="segment"></param>
        /// <returns></returns>
        public CollisionResponse CollidingWith(SegmentCollider segment)
        {
            var poi = LinearUtils.SegmentIntersectionPoint(
                Start.X, Start.Y, End.X, End.Y, segment.Start.X,
                segment.Start.Y, segment.End.X, segment.End.Y
                );
            var c_res = new CollisionResponse(this, segment, poi.HasValue);
            if (!c_res.Colliding)
                return c_res;
            c_res.SetAttr<Vector2>("IntersectionPoint", poi.Value);
            return c_res;
        }
    }
}
