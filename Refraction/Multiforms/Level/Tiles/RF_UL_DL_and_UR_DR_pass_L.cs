﻿#region License

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

using System.Collections.Generic;

#endregion

namespace Refraction_V2.Multiforms.Level.Tiles
{
    public class RF_UL_DL_and_UR_DR_pass_L : ComplexRefractorTile
    {

        public override TileType Type { get { return TileType.RF_UL_DL_and_UR_DR_pass_L; } }

        private static readonly List<Directions> PASS_THROUGH
            = new List<Directions>() { Directions.Right, Directions.Left };

        private static readonly Dictionary<Directions, Directions> INPUT_OUTPUT_MAPPING_LEFT
            = new Dictionary<Directions, Directions>()
        {
            { Directions.UpLeft, Directions.DownLeft },
            { Directions.UpRight, Directions.DownRight }
        };

        private static readonly Dictionary<Directions, Directions> INPUT_OUTPUT_MAPPING_RIGHT
            = new Dictionary<Directions, Directions>()
        {
            { Directions.DownLeft, Directions.UpLeft },
            { Directions.DownRight, Directions.UpRight }
        };

        public override List<Directions> PassThrough { get { return PASS_THROUGH; } }

        public override Dictionary<Directions, Directions> InputOutputMappingLeft { get { return INPUT_OUTPUT_MAPPING_LEFT; } }

        public override Dictionary<Directions, Directions> InputOutputMappingRight { get { return INPUT_OUTPUT_MAPPING_RIGHT; } }

        public RF_UL_DL_and_UR_DR_pass_L(Vector2 position, bool open)
            : base(Assets.Level.Images.Refractor_UL_DL_and_UR_DR_pass_L, position, open) { }

    }
}
