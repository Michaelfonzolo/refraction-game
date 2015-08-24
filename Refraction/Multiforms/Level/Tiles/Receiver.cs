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
 * A level end controller tile representing a static receiver for a laser.
 */

#endregion

#region Using Statements

using DemeterEngine;
using DemeterEngine.Graphics;
using Microsoft.Xna.Framework;
using System;

#endregion

namespace Refraction_V2.Multiforms.Level.Tiles
{
    public class Receiver : LevelEndController
    {

		/// <summary>
		/// The direction the laser must enter the tile to activate it.
		/// </summary>
        public Directions InputDirection { get; private set; }

		/// <summary>
		/// The colour the input laser must be to activate it.
		/// </summary>
        public LaserColours InputColour { get; private set; }

		/// <summary>
		/// The sprite representing this tile.
		/// </summary>
        public Sprite TileSprite { get; private set; }

        public Receiver(Vector2 position, Directions direction, LaserColours colour)
            : base(position, false) // Receivers are never open tiles.
        {
            InputDirection = direction;
            InputColour = colour;

            TileSprite = new Sprite(Assets.Level.Images.Receiver);
            TileSprite.CenterOn(Center);
            TileSprite.Rotation = Math.Atan2(InputDirection.Y, -InputDirection.X);
            TileSprite.Tint = InputColour.Color * 3f;
        }

        public override void UpdateLaser(BoardForm board, Laser laser, Point point)
        {
            if (laser.CurrentDirection == InputDirection &&
                laser.CurrentColour == InputColour)
                Activated = true;
            laser.SetSegmentEnd(Center);
            laser.Kill();
        }

        public override void Render()
        {
            base.Render();
            TileSprite.Render();
        }

    }
}
