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
 */

#endregion

#region Using Statements

using DemeterEngine.Multiforms.Forms;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Refraction_V2.Utils;

#endregion

namespace Refraction_V2.Multiforms
{
    /// <summary>
    /// A child form of SimpleTextForm that implements ITransitionalForm to be compatible
    /// with the common effectors (in Refraction_V2.Multiforms.Effectors).
    /// </summary>
    public class TextForm : SimpleTextForm, ITransitionalForm
    {

        public TextForm(
            string textForm, SpriteFont font, Vector2 position, Color colour, bool centred = true)
            : base(textForm, font, position, centred)
        {
            Tint = colour;
        }

        public void SetAlpha(float alpha)
        {
            Alpha = alpha;
            // Console.WriteLine(Alpha);
        }

        public Vector2 GetPosition(PositionType positionType)
        {
            var dimensions = Font.MeasureString(Text);
            var fromType = Centred ? PositionType.Center : PositionType.TopLeft;
            return PositionConverter.ToType(Position, dimensions.X, dimensions.Y, fromType, positionType);
        }

        public void SetPosition(Vector2 vec, PositionType positionType)
        {
            var dimensions = Font.MeasureString(Text);
            var toType = Centred ? PositionType.Center : PositionType.TopLeft;
            Position = PositionConverter.ToType(vec, dimensions.X, dimensions.Y, positionType, toType);
        }
    }
}
