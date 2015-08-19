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
 * A builtin Collidable subclass representing a point in 2D space.
 * 
 * NOTE: Despite the name similarity between PointCollider, System.Drawing.Point, and
 * Microsoft.Xna.Framework.Point, PointCollider's abcissa and ordinate are doubles, 
 * not ints.
 */

#endregion

#region Using Statements

using Microsoft.Xna.Framework;
using DemeterEngine.Maths;
using System;

#endregion

namespace DemeterEngine.Collision
{
    public class PointCollider : Collidable
    {

        /// <summary>
        /// The PointCollider's precedence (0).
        /// </summary>
        public override int Precedence { get { return CollisionPrecedences.POINT; } }

        /// <summary>
        /// The center of this point (the point itself).
        /// </summary>
        public Vector2 Center { get { return new Vector2((float)X, (float)Y); } }

        /// <summary>
        /// The area of this point (zero).
        /// </summary>
        public double Area { get { return 0; } }

        /// <summary>
        /// The perimeter of this point (zero).
        /// </summary>
        public double Perimeter { get { return 0; } }

        /// <summary>
        /// The x coordinate of this point.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// The y coordinate of this point.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// This point's bounding box (a box with zero width and height).
        /// </summary>
        /// <returns></returns>
        public override double[] BoundingBox() { return new double[] { X, Y, 0, 0 }; }

        public PointCollider()
            : this(0, 0) { }

        public PointCollider(double c)
            : this(c, c) { }

        public PointCollider(Vector2 vec)
            : this(vec.X, vec.Y) { }

        public PointCollider(Point point)
            : this(point.X, point.Y) { }

        public PointCollider(PointCollider point)
            : this(point.X, point.Y) { }

        public PointCollider(double x, double y)
        {
            X = x;
            Y = y;
        }

        public PointCollider(double[] args)
        {
            var l = args.Length;
            switch (l)
            {
                case 0:
                    X = 0;
                    Y = 0;
                    break;
                case 1:
                    X = args[0];
                    Y = args[0];
                    break;
                case 2:
                    X = args[0];
                    Y = args[1];
                    break;
                default:
                    throw new ArgumentException(
                        String.Format(
                            "Invalid argument count {0} for type \"PointCollider\"", l)
                            );
            }
        }

        /// <summary>
        /// Set the position of this PointCollider.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetPosition(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Set the position of this PointCollider.
        /// </summary>
        /// <param name="pos"></param>
        public void SetPosition(Vector2 pos)
        {
            X = pos.X;
            Y = pos.Y;
        }

        /// <summary>
        /// Translate this PointCollider by the given amount.
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        public void Translate(double dx, double dy)
        {
            X += dx;
            Y += dy;
        }

        /// <summary>
        /// Translate this PointCollider by the given amount.
        /// </summary>
        /// <param name="delta"></param>
        public void Translate(Vector2 delta)
        {
            X += delta.X;
            Y += delta.Y;
        }

        /// <summary>
        /// Rotate this point by the given amount relative to the given point.
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="origin"></param>
        /// <param name="degrees"></param>
        /// <param name="absoluteOrigin"></param>
        public void Rotate(double angle, Vector2 origin, bool degrees = true, bool absoluteOrigin = true)
        {
            SetPosition(VectorUtils.Rotate(Center, angle, origin, degrees, absoluteOrigin));
        }

        private bool _eq(double x, double y)
        {
            return X == x && Y == y;
        }

        /// <summary>
        /// Check if this PointCollider is colliding with the given PointCollider.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public CollisionResponse CollidingWith(PointCollider other)
        {
            return new CollisionResponse(this, other, _eq(other.X, other.Y));
        }

        /// <summary>
        /// Check if this PointCollider is colliding with the given Vector2.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public CollisionResponse CollidingWith(Vector2 point)
        {
            return CollidingWith(new PointCollider(point));
        }

        /// <summary>
        /// Check if this PointCollider is colliding with the given Point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public CollisionResponse CollidingWith(Point point)
        {
            return CollidingWith(new PointCollider(point));
        }

    }
}
