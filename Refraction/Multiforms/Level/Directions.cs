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
 * An "enum" representing the different directions a laser can move in.
 * 
 * It's an enum in the java-sense of the word. C#'s enum's suck.
 */

#endregion

#region Using Statements

using Microsoft.Xna.Framework;

using System.Collections.Generic;

#endregion

namespace Refraction_V2.Multiforms.Level
{
    public class Directions
    {

		// The x and y coordinates of a direction indicate how to move around the multidimensional array
		// of level tiles. For example, if a laser's current position is (x, y) in the 2d-array, and it
		// moves up-left, then it's next position in the 2d-array is (x - 1, y + 1).
		//
		// A consequence of this is that the directions don't necessarily represent how to draw a laser
 		// in the game world, since the y-axis is flipped. To clarify with an example, the UpLeft direction
		// looks like DownLeft when rendered. To mitigate this, simply flip the y-coordinate whenever using
		// a direction for graphical purposes.

        public int X { get; private set; }
        public int Y { get; private set; }

        private Directions(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Vector2 ToVec()
        {
            return new Vector2(X, Y);
        }

        public static Point operator +(Point point, Directions direction)
        {
            return new Point(point.X + direction.X, point.Y + direction.Y);
        }

        public static Vector2 operator +(Vector2 vector, Directions direction)
        {
            return new Vector2(vector.X + direction.X, vector.Y + direction.Y);
        }

        public static readonly Directions Up    = new Directions(0, 1);
        public static readonly Directions Down  = new Directions(0, -1);
        public static readonly Directions Right = new Directions(1, 0);
        public static readonly Directions Left  = new Directions(-1, 0);

        public static readonly Directions UpRight   = new Directions(1, 1);
        public static readonly Directions UpLeft    = new Directions(-1, 1);
        public static readonly Directions DownRight = new Directions(1, -1);
        public static readonly Directions DownLeft  = new Directions(-1, -1);

		/// <summary>
		/// The dictionary of direction instances by their name.
		/// 
		/// Unfortunately, since this isn't actually an enum, it just sorta acts like one,
		/// we can't say Enum.Parse(Directions, name) to turn a name into a direction. This
		/// directionary is the alternative.
		/// </summary>
        public static readonly Dictionary<string, Directions> DirectionsByName = new Dictionary<string, Directions>()
        {
            {"Up",    Up},
            {"Down",  Down},
            {"Right", Right},
            {"Left",  Left},

            {"UpRight",   UpRight},
            {"UpLeft",    UpLeft},
            {"DownRight", DownRight},
            {"DownLeft",  DownLeft}
        };

    }
}
