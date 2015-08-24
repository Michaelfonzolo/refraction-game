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
 * A tile representing a basic refractor tile. The basic refractor tiles are the 8
 * refractors that have two input directions and a single output direction. Every other
 * refractor requires special handling.
 */

#endregion

#region Using Statements

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

#endregion

namespace Refraction_V2.Multiforms.Level.Tiles
{
    public abstract class BasicRefractorTile : RefractorTile
    {

		/// <summary>
		/// The directions light can enter.
		/// </summary>
        protected abstract Directions[] ValidInputDirections { get; }

		/// <summary>
		/// The output direction of light.
		/// </summary>
        protected abstract Directions OutputDirection { get; }

		/// <summary>
		/// Whether or not this refractor is in use (i.e. a laser is passing through it).
		/// Basic refractors can only refract a single laser colour at a time.
		/// </summary>
		public bool InUse { get; protected set; }

        public BasicRefractorTile(Texture2D texture, Vector2 position, bool open)
            : base(texture, position, open) { }

        public override void Update()
        {
            base.Update();
            InUse = false;
        }

        public override void UpdateLaser(BoardForm board, Laser laser, Point point)
        {
            laser.SetSegmentEnd(Center);

            if (InUse || !ValidInputDir(laser))
            {
                laser.Kill();
                return;
            }

            InUse = true;
            laser.SetSegmentStart(Center);
            laser.SetDirection(OutputDirection);
        }

        private bool ValidInputDir(Laser l)
        {
            return ValidInputDirections.Contains(l.CurrentDirection);
        }

    }
}
