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

/* 
 * Author: Michael Ala
 */

#endregion

#region Using Statements

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.Collections.Generic;

#endregion

namespace Refraction_V2.Multiforms.Level.Tiles
{

    /// <summary>
    /// A complex refractor tile is the base type for the refractors that refract lasers
    /// orthogonally and allow another laser to pass through it.
    /// </summary>
    public abstract class ComplexRefractorTile : RefractorTile
    {

        // Complex refractor tiles have two sides, which can unambiguously be labelled
        // the "left" and "right" sides. Each one can be used individually by different
        // lasers, so they require separate "InUse" flags.
        private bool InUse_Left = false, InUse_Right = false;

        // Note: It doesn't matter whether the "left" and "right" variables actually contain
        // information about the physical left and right sides of the tile. All that matters
        // is that all the information for one side is assigned one name ("left" or "right"),
        // and the other side the other name to distinguish.

        /// <summary>
        /// The mapping of input directions to output directions on the "left" side of the
        /// complex refractor.
        /// </summary>
        public abstract Dictionary<Directions, Directions> InputOutputMappingLeft { get; }

        /// <summary>
        /// The mapping of input directions to output directions on the "right" side of the
        /// complex refractor.
        /// </summary>
        public abstract Dictionary<Directions, Directions> InputOutputMappingRight { get; }

        /// <summary>
        /// The list of directions for which the laser can pass through the tile.
        /// </summary>
        public abstract List<Directions> PassThrough { get; }

        public ComplexRefractorTile(Texture2D texture, Vector2 position, bool open)
            : base(texture, position, open) { }

        public override void Update()
        {
            base.Update();
            InUse_Left = false;
            InUse_Right = false;
        }

        public override void UpdateLaser(BoardForm board, Laser laser, Point point)
        {
            var dir = laser.CurrentDirection;
            if (PassThrough.Contains(dir))
            {
                return;
            }

            laser.SetSegmentEnd(Center);

            if (!InUse_Left && InputOutputMappingLeft.ContainsKey(dir))
            {
                laser.SetSegmentStart(Center);
                laser.SetDirection(InputOutputMappingLeft[dir]);
                InUse_Left = true;
            }
            else if (!InUse_Right && InputOutputMappingRight.ContainsKey(dir))
            {
                laser.SetSegmentStart(Center);
                laser.SetDirection(InputOutputMappingRight[dir]);
                InUse_Right = true;
            }
            else
            {
                laser.Kill();
            }
        }

    }
}
