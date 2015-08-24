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
 * A LevelTile which outputs a laser.
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
    public class Outputter : LevelTile
    {

		/// <summary>
		/// The direction to output the laser.
		/// </summary>
        public Directions OutputDirection { get; private set; }

		/// <summary>
		/// The colour of the output laser.
		/// </summary>
        public LaserColours OutputColour { get; private set; }

		/// <summary>
		/// The sprite representing this tile.
		/// </summary>
        public Sprite TileSprite { get; private set; }

        public Outputter(Vector2 position, Directions direction, LaserColours colour)
            : base(position, false) // Outputters are never open tiles.
        {
            OutputDirection = direction;
            OutputColour = colour;

            TileSprite = new Sprite(Assets.Level.Images.Outputter);
            TileSprite.CenterOn(Center);
            TileSprite.Rotation = Math.Atan2(OutputDirection.Y, OutputDirection.X);
            TileSprite.Tint = OutputColour.Color * 3f;
        }

        public override void UpdateLaser(BoardForm board, Laser laser, Point point)
        {
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
