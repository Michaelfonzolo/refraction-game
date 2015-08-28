using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DemeterEngine.Multiforms.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Refraction_V2.Utils;

namespace Refraction_V2.Multiforms
{
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
