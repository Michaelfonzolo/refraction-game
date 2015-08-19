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
 * An object representing a response to a collision query between two Collidable
 * objects. It stores the two objects involved in the collision, and whether or
 * not they are colliding. The object is an AttributeContainer, so it also contains
 * information relevant to the specific collision (i.e. intersection points, etc.).
 */

#endregion

#region Using Statements

using DemeterEngine.Utils;

#endregion

namespace DemeterEngine.Collision
{
    public class CollisionResponse : AttributeContainer
    {

        /// <summary>
        /// The first of the two Collidable objects involved.
        /// </summary>
        public Collidable ColliderA { get; private set; }

        /// <summary>
        /// The second of the two Collidable objects involved.
        /// </summary>
        public Collidable ColliderB { get; private set; }

        /// <summary>
        /// Whether or not the two objects are colliding.
        /// </summary>
        public bool Colliding { get; set; }

        public CollisionResponse(Collidable a, Collidable b, bool colliding)
        {
            ColliderA = a;
            ColliderB = b;
            Colliding = colliding;
        }

    }
}
