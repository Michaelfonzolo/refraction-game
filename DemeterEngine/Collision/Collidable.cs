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
 * Collidable is an abstract base class used to implement objects that represent
 * collidable geometries. Each Collidable type must have a Precedence. If a Collidable
 * type's Precedence is N, it is required to implement a CollidingWith(T obj) method
 * for every Collidable subclass T such that T's precedence is less than or equal to N.
 * 
 * Every CollidingWith method must return a CollisionResponse object, representing the
 * result of the collision.
 * 
 * This file also includes an InternalChecker class that runs dynamic checking between
 * Collidable objects. By `dynamic`, I mean it uses duck-typing. If the input objects
 * behave like Collidable objects (i.e. have CollidingWith methods), then they ARE
 * Collidable objects as far as the checker is concerned.
 */

#endregion

#region Using Statements

using System;
using Microsoft.CSharp.RuntimeBinder;
using DemeterEngine.Maths;

#endregion

namespace DemeterEngine.Collision
{
    public abstract class Collidable
    {

        private class InternalChecker
        {
            /// <summary>
            /// Check for a collision between two Collidable objects.
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <returns></returns>
            public static CollisionResponse CollisionBetween(Collidable a, Collidable b)
            {
                try
                {
                    var abb = a.BoundingBox();
                    var bbb = b.BoundingBox();
                    // If their bounding boxes don't collide, the geometries themselves
                    // can't be colliding either.
                    if (!RectUtils.Collision(
                            abb[0], abb[1], abb[2], abb[3],
                            bbb[0], bbb[1], bbb[2], bbb[3]))
                        return new CollisionResponse(a, b, false);
                    if (a.Precedence >= b.Precedence)
                        return DynamicCollisionBetween(a, b);
                    return DynamicCollisionBetween(b, a);
                }
                catch (RuntimeBinderException)
                {
                    // This occurs when the object with higher precedence doesnt implement
                    // a CollidingWith method for the object with the lower precedence.
                    return new CollisionResponse(a, b, false);
                }
            }

            /// <summary>
            /// Check for a collision between a Collidable object and another object of arbitrary
            /// type. This allows Collidable subclasses to implement collision methods for objects
            /// that aren't necessarily Collidable objects (for example, PointCollider implements
            /// a collision method for Point and Vector2, even though they don't inherit from
            /// Collidable).
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <returns></returns>
            public static CollisionResponse CollisionBetween(Collidable a, object b)
            {
                try
                {
                    return DynamicCollisionBetween(a, b);
                }
                catch (RuntimeBinderException)
                {
                    return null;
                }
            }

            private static CollisionResponse DynamicCollisionBetween(dynamic a, dynamic b)
            {
                // This is to prevent infinite recursion in the rare case that
                // CollisionBetween(a, new object()) is called.
                if (b == new object())
                    return null;
                return a.CollidingWith(b);
            }
        }

        /// <summary>
        /// The precedence of this Collidable object. Objects with higher precedences are assumed
        /// to have CollidingWith methods implemented for all objects with precedences lower
        /// precedences.
        /// 
        /// Specifically, if Collidable subtype A as precedence M, then it is assumed to implement
        /// CollidingWith methods for all Collidable subtypes B whose precedence N is such that
        /// N <= M.
        /// </summary>
        public abstract int Precedence { get; }

        // I would make it return a Rectangle, but those only use integer coordinates
        // (fuckin' so stupid), so an array of doubles is desired instead.

        /// <summary>
        /// An array of doubles representing the X, Y, W, and H of the geometry's
        /// bounding rectangle. (X & Y being the coordinates of the top left corner,
        /// and W & H being the width & height respectively).
        /// </summary>
        /// <returns></returns>
        public abstract double[] BoundingBox();

        /// <summary>
        /// Check for a collision between this Collidable object and another.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public CollisionResponse CollidingWith(Collidable other)
        {
            return InternalChecker.CollisionBetween(this, other);
        }

        /// <summary>
        /// Check for a collision between this Collidable object and another (of arbitrary type).
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public CollisionResponse CollidingWith(object other)
        {
            return InternalChecker.CollisionBetween(this, other);
        }

    }
}
