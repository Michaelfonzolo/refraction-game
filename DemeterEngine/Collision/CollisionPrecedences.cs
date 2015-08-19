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
 * The precedences of the builtin Collidable objects. This also includes the maximum
 * internal precedence, so that you know what the lowest precedence you can use is if
 * you wish to implement your own Collidable subclass that is compatible with all the
 * builtin ones.
 */

#endregion

namespace DemeterEngine.Collision
{
    public static class CollisionPrecedences
    {

        internal const int POINT = 0;
        internal const int SEGMENT = 1;
        internal const int RECT = 2;
        internal const int ELLIPSE = 3;
        /*
        internal const int CONVEX_POLYGON = 4;
        internal const int POLYGON = 5;
         */

        public static readonly int MAX = ELLIPSE;

    }
}
