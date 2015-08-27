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
 * The laser class, representing a laser shot out from an outputter. The job of the
 * actual laser class is to trace it's path through the multidimensional array of
 * level tiles, and render itself.
 */

#endregion

#region Using Statements

using DemeterEngine;
using DemeterEngine.Maths;
using DemeterEngine.Multiforms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

#endregion

namespace Refraction_V2.Multiforms.Level
{
    public class Laser : Form
    {

		/// <summary>
		/// The current direction the laser is heading in. This is used during the
		/// TracePath method to determine how to navigate the 2d-array of level tiles.
		/// </summary>
        public Directions CurrentDirection { get; private set; }

		/// <summary>
		/// The current colour of the laser. This is used during the TracePath method.
		/// 
		/// The name 'CurrentColour' is in reference to the future possibility of tiles
		/// that change the colour of a laser without kiling it.
		/// </summary>
        public LaserColours CurrentColour { get; private set; }

		/// <summary>
		/// Whether or not this laser is dead (i.e. has hit a wall or gone offscreen).
		/// This is used by the TracePath method to determine when to stop tracing.
		/// </summary>
        private bool Killed;

		/// <summary>
		/// The start position of the current laser segment.
		/// </summary>
        private Vector2 CurrentSegmentStart;

		/// <summary>
		/// The list of laser segments traced out by the TracePath method.
		/// </summary>
        private List<LaserSegment> Segments = new List<LaserSegment>();

        public Laser(Directions initialDirection, LaserColours initialColour, int initialFrame)
            : base(true, initialFrame)
        {
            CurrentDirection = initialDirection;
            CurrentColour = initialColour;
            Killed = false;
        }

		/// <summary>
		/// Trace the path of this laser from the given position on the board.
		/// </summary>
        public void TracePath(BoardForm boardForm, Point point)
        {
            var initialTile = boardForm.Board[point.Y, point.X];
            CurrentSegmentStart = initialTile.Center;

            while (true)
            {
                point += CurrentDirection;
                if (!IsValidPosition(boardForm, point))
                {
                    Segments.Add(
                        new LaserSegment(
                            CurrentSegmentStart,
                            CurrentSegmentStart + 2000 * new Vector2(CurrentDirection.X, -CurrentDirection.Y),
                            // 2000 should be enough to make the laser go offscreen
                            // from anywhere on the board.
                            // Also, we have to invert the direction's Y value because
                            // the screen is flipped on the y-axis and these directions
                            // refer to how the laser traverses the array of tiles, instead
                            // of the actual physical space.
                            CurrentColour.Color
                            )
                        );
                    break;
                }

                var nextTile = boardForm.Board[point.Y, point.X];
                nextTile.UpdateLaser(boardForm, this, point);

                if (Killed)
                    return;
            }
        }

		/// <summary>
		/// Set the current direction of the laser. This method is used during
		/// the TracePath method.
		/// </summary>
		/// <param name="direction"></param>
        public void SetDirection(Directions direction)
        {
            CurrentDirection = direction;
        }

		/// <summary>
		/// Set the next segment start position of the laser. This method is used
		/// during the TracePath method.
		/// </summary>
		/// <param name="segmentStart"></param>
        public void SetSegmentStart(Vector2 segmentStart)
        {
            CurrentSegmentStart = segmentStart;
        }

		/// <summary>
		/// Set the next segment end position of the laser. This method is used
		/// during the TracePath method.
		/// </summary>
		/// <param name="segmentEnd"></param>
        public void SetSegmentEnd(Vector2 segmentEnd)
        {
            Segments.Add(
                new LaserSegment(
                    CurrentSegmentStart,
                    segmentEnd,
                    CurrentColour.Color
                    )
                );
        }

		/// <summary>
		/// Kill the laser. This method is used during the TracePath method.
		/// </summary>
        public void Kill()
        {
            Killed = true;
        }
        
		/// <summary>
		/// Check if a position is a valid position on the board.
		/// </summary>
        private bool IsValidPosition(BoardForm boardForm, Point point)
        {
            return 0 <= point.X && point.X < boardForm.Dimensions.X &&
                   0 <= point.Y && point.Y < boardForm.Dimensions.Y;
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Render()
        {
            var scale = 0.2f * (float)SpecialFunctions.SineSquared(LocalFrame / 50f) + 0.5f;
            foreach (var segment in Segments)
            {
                segment.Render(scale);
            }
        }

    }
}
