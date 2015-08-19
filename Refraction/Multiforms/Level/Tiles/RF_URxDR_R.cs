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
 * A basic refractor that takes input from lasers going up-right or down-right
 * and outputs a laser of the same colour going right.
 */

#endregion

#region Using Statements

using Microsoft.Xna.Framework;

#endregion

namespace Refraction_V2.Multiforms.Level.Tiles
{
    public class RF_URxDR_R : BasicRefractorTile
    {

        public override TileType Type { get { return TileType.RF_URxDR_R; } }

        protected override Directions[] ValidInputDirections
        {
            get 
            { 
                return new Directions[] 
                { 
                    Directions.UpRight, 
                    Directions.DownRight 
                }; 
            }
        }

        protected override Directions OutputDirection { get { return Directions.Right; } }

        public RF_URxDR_R(Vector2 position, bool open)
            : base("Refractor_URxDR_R", position, open) { }
    }
}
