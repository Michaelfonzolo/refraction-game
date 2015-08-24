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
 * A builtin Collidable subclass representing an axis-oriented rectangle.
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
    public class RectCollider : Collidable
    {

        /// <summary>
        /// The RectCollider's precedence (2).
        /// </summary>
        public override int Precedence { get { return CollisionPrecedences.RECT; } }

        // The private internal caches for the center and the coordinates of this rectangle.
        private PropertyCache<Vector2> centerCache = new PropertyCache<Vector2>();
        private PropertyCache<Vector2[]> coordsCache = new PropertyCache<Vector2[]>();

        /// <summary>
        /// The center of this rectangle.
        /// </summary>
        public Vector2 Center
        {
            get
            {
                if (centerCache.dirty)
                    centerCache.Clean = new Vector2((float)(X + W / 2.0), (float)(Y + H / 2.0));
                return centerCache.Value;
            }
        }

        /// <summary>
        /// The vector representing the center on the top edge of the rectangle.
        /// </summary>
        public Vector2 TopCenter { get { return new Vector2((float)(X + W / 2f), (float)Y); } }

        /// <summary>
        /// The vector representing the center on the bottom edge of the rectangle.
        /// </summary>
        public Vector2 BottomCenter { get { return new Vector2((float)(X + W / 2f), (float)(Y + H)); } }

        /// <summary>
        /// The vector representing the center on the left edge of the rectangle.
        /// </summary>
        public Vector2 LeftCenter { get { return new Vector2((float)X, (float)(Y + H / 2f)); } }

        /// <summary>
        /// The vector representing the center on the bottom edge of the rectangle.
        /// </summary>
        public Vector2 RightCenter { get { return new Vector2((float)(X + W), (float)(Y + H / 2f)); } }

        /// <summary>
        /// The coordinates of this rectangle, ordered clockwise (relative to the game world)
        /// starting with the top left corner.
        /// </summary>
        public Vector2[] Coords
        {
            get
            {
                if (coordsCache.dirty)
                    coordsCache.Clean = new Vector2[]
                    {
                        new Vector2((float)X, (float)Y),
                        new Vector2((float)(X + W), (float)Y),
                        new Vector2((float)(X + W), (float)(Y + H)),
                        new Vector2((float)X, (float)(Y + H))
                    };
                return coordsCache.Value;
            }
        }

        /// <summary>
        /// The top left corner of this rectangle (relative to the game world).
        /// </summary>
        public Vector2 TopLeft { get { return Coords[0]; } }

        /// <summary>
        /// The top right corner of this rectangle (relative to the game world).
        /// </summary>
        public Vector2 TopRight { get { return Coords[1]; } }

        /// <summary>
        /// The bottom right corner of this rectangle (relative to the game world).
        /// </summary>
        public Vector2 BottomRight { get { return Coords[2]; } }

        /// <summary>
        /// The bottom left corner of this rectangle (relative to the game world).
        /// </summary>
        public Vector2 BottomLeft { get { return Coords[3]; } }

        /// <summary>
        /// The area of this rectangle.
        /// </summary>
        public double Area { get { return W * H; } }

        /// <summary>
        /// The perimeter of this rectangle.
        /// </summary>
        public double Perimeter { get { return 2 * (W + H); } }

        /// <summary>
        /// The x coordinate of the top left corner of this rectangle.
        /// </summary>
        public double X { get; private set; }

        /// <summary>
        /// The y coordinate of the top left corner of this rectangle.
        /// </summary>
        public double Y { get; private set; }

        /// <summary>
        /// The width of this rectangle.
        /// </summary>
        public double W { get; private set; }

        /// <summary>
        /// The height of this rectangle.
        /// </summary>
        public double H { get; private set; }

		/// <summary>
		/// The y-value of the top of this rectangle (relative to the game world).
		/// </summary>
		public double Top { get { return Y; } }

		/// <summary>
		/// The y-value of the bottom of this rectangle (relative to the game world).
		/// </summary>
		public double Bottom { get { return Y + H; } }

		/// <summary>
		/// The x-value of the left side of this rectangle.
		/// </summary>
		public double Left { get { return X; } }

		/// <summary>
		/// The x-value of the right side of this rectangle.
		/// </summary>
		public double Right { get { return X + W; } }

        /// <summary>
        /// This rectangle's bounding box (lol gee I wonder what it is).
        /// </summary>
        /// <returns></returns>
        public override double[] BoundingBox()
        {
            return new double[] { X, Y, W, H };
        }

        public RectCollider()
            : this(0, 0, 0, 0) { }

        public RectCollider(double l)
            : this(0, 0, l, l) { }

        public RectCollider(double w, double h)
            : this(0, 0, w, h) { }

        public RectCollider(double x, double y, double l)
            : this(x, y, l, l) { }

        public RectCollider(double x, double y, double w, double h)
        {
            X = x;
            Y = y;
            W = w;
            H = h;
        }

        public RectCollider(Vector2 position, double w, double h)
            : this(position.X, position.Y, w, h) { }

        public RectCollider(Vector2 position, Vector2 sides)
            : this(position.X, position.Y, sides.X, sides.Y) { }

        public RectCollider(RectCollider rect)
            : this(rect.X, rect.Y, rect.W, rect.H) { }

        public RectCollider(Rectangle rect)
            : this(rect.X, rect.Y, rect.Width, rect.Height) { }

        public RectCollider(double[] args)
        {
            var l = args.Length;
            switch (l)
            {
                case 0:
                    X = 0;
                    Y = 0;
                    W = 0;
                    H = 0;
                    break;
                case 1:
                    X = 0;
                    Y = 0;
                    W = args[0];
                    H = args[0];
                    break;
                case 2:
                    X = 0;
                    Y = 0;
                    W = args[0];
                    H = args[1];
                    break;
                case 3:
                    X = args[0];
                    Y = args[1];
                    W = args[2];
                    H = args[2];
                    break;
                case 4:
                    X = args[0];
                    Y = args[1];
                    W = args[2];
                    H = args[3];
                    break;
                default:
                    throw new ArgumentException(
                        String.Format(
                            "Invalid argument count {0} for type \"RectCollider\"", l)
                            );
            }
        }

        /// <summary>
        /// Set the top left corner of this rectangle.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetPosition(double x, double y)
        {
            X = x;
            Y = y;
            // As tempting as it is to want to just set centerCache.Value to the
            // new center right now, that defeats it's purpose.
            centerCache.dirty = true;
            coordsCache.dirty = true;
        }

        /// <summary>
        /// Set the top left corner of this rectangle.
        /// </summary>
        /// <param name="pos"></param>
        public void SetPosition(Vector2 pos)
        {
            X = pos.X;
            Y = pos.Y;
            centerCache.dirty = true;
            coordsCache.dirty = true;
        }

        /// <summary>
        /// Translate this rectangle by the given amount.
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        public void Translate(double dx, double dy)
        {
            X += dx;
            Y += dy;
            var c = centerCache.Value;
            centerCache.dirty = true;
            coordsCache.dirty = true;
        }

        /// <summary>
        /// Translate this rectangle by the given amount.
        /// </summary>
        /// <param name="delta"></param>
        public void Translate(Vector2 delta)
        {
            X += delta.X;
            Y += delta.Y;
            centerCache.dirty = true;
            coordsCache.dirty = true;
        }

        /// <summary>
        /// Rotate this RectCollider by the given amount relative to its center.
        /// 
        /// Since RectColliders are axis-oriented, this just throws an error.
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="degrees"></param>
        public void Rotate(double angle, bool degrees = true)
        {
            throw new NotImplementedException("Cannot rotate RectCollider.");
        }

        /// <summary>
        /// Rotate this RectCollider by the given amount relative to the given point.
        /// 
        /// Since RectColliders are axis-oriented, this just throws an error.
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="origin"></param>
        /// <param name="degrees"></param>
        /// <param name="relative"></param>
        public void Rotate(double angle, Vector2 origin, bool degrees = true, bool relative = true)
        {
            throw new NotImplementedException("Cannot rotate RectCollider.");
        }

        /// <summary>
        /// Scale this RectCollider by the given amount relative to its center.
        /// </summary>
        /// <param name="amount"></param>
        public void Scale(double amount)
        {
            var pW = W;
            var pH = H;
            W *= amount;
            Y *= amount;
            X -= (W - pW) / 2.0;
            Y -= (H - pH) / 2.0;
        }

        /// <summary>
        /// Scale this RectCollider by the given amount relative to the given point.
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="origin"></param>
        /// <param name="relative"></param>
        public void Scale(double amount, Vector2 origin, bool relative = true)
        {
            if (relative)
                origin += TopLeft;
            double dx = origin.X - X;
            double dy = origin.Y - Y;
            double alpha = 1 - amount;
            X -= alpha * dx;
            Y -= alpha * dy;
            W *= amount;
            H *= amount;
        }

        /// <summary>
        /// Check if this RectCollider is colliding with the given PointCollider.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public CollisionResponse CollidingWith(PointCollider point)
        {
            bool inside = X <= point.X && point.X <= X + W &&
                          Y <= point.Y && point.Y <= Y + H;
            return new CollisionResponse(this, point, inside);
        }

        /// <summary>
        /// Check if this RectCollider is colliding with the given Vector2.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public CollisionResponse CollidingWith(Vector2 point)
        {
            return CollidingWith(new PointCollider(point));
        }

        /// <summary>
        /// Check if this RectCollider is colliding with the given Point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public CollisionResponse CollidingWith(Point point)
        {
            return CollidingWith(new PointCollider(point));
        }

        /// <summary>
        /// Check if this RectCollider is colliding with the given SegmentCollider.
        /// </summary>
        /// <param name="segment"></param>
        /// <returns></returns>
        public CollisionResponse CollidingWith(SegmentCollider segment)
        {
            var intersections = new List<Vector2>();
            var corners = Coords;
            Vector2 p1, p2;
            Vector2? poi;
            for (int i = 0; i < 4; i++)
            {
                p1 = corners[(i - 1) % 4];
                p2 = corners[i];
                poi = LinearUtils.SegmentIntersectionPoint(
                    p1.X, p1.Y, p2.X, p2.Y,
                    segment.Start.X, segment.Start.Y,
                    segment.End.X, segment.End.Y
                    );
                if (poi.HasValue)
                    intersections.Add(poi.Value);
            }
            var c_res = new CollisionResponse(this, segment, intersections.Count > 0);
            if (!c_res.Colliding)
                return c_res;
            c_res.SetAttr<Vector2[]>("Intersections", intersections.ToArray());
            return c_res;
        }

        /// <summary>
        /// Check if this RectCollider is colliding with the given RectCollider.
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public CollisionResponse CollidingWith(RectCollider rect)
        {
            return new CollisionResponse(this, rect, RectUtils.Collision(
                X, Y, W, H, rect.X, rect.Y, rect.W, rect.H));
        }

    }
}
