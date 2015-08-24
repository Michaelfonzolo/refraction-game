using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DemeterEngine;
using DemeterEngine.Collision;
using DemeterEngine.Graphics;
using DemeterEngine.Multiforms.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Refraction_V2.Utils;

namespace Refraction_V2.Multiforms
{
    public class GUIButton : ButtonForm
    {

        /// <summary>
        /// The default colour used by GUIButtons.
        /// </summary>
        public static readonly Color DefaultTextColour = Color.Black;

        public static readonly Vector2 TextPositionOffset = new Vector2(0, 5);

        /// <summary>
        /// The GUIButtonInfo describing this button.
        /// </summary>
        public GUIButtonInfo Info { get; private set; }

        /// <summary>
        /// The sprite representing this button.
        /// </summary>
        public Sprite ButtonSprite { get; private set; }

        /// <summary>
        /// The position of the text relative to the button.
        /// </summary>
        public Vector2 TextPosition { get; private set; }

        /// <summary>
        /// The colour of the text.
        /// </summary>
        public Color TextColour { get; private set; }

        public SpriteFont Font { get; private set; }

        public GUIButton(GUIButtonInfo info, Vector2 position, PositionType positionType = PositionType.Center)
            : base(null)
        {
            Info = info;
            Font = info.Font;

            ButtonSprite = new Sprite(info.Texture);
            var width = ButtonSprite.Width;
            var height = ButtonSprite.Height;
            var center = PositionConverter.ToCenter(position, width, height, positionType);

            ButtonSprite.CenterOn(center);

            Collider = new RectCollider(
                center - ButtonSprite.TextureCenter,
                ButtonSprite.Width, ButtonSprite.Height);

            TextPosition = center - Font.MeasureString(info.Text) / 2f + TextPositionOffset;
            TextColour = info.InitialTextColor.HasValue ? info.InitialTextColor.Value 
                                                        : DefaultTextColour;
        }

        public override void Render()
        {
            ButtonSprite.Tint = CollidingWithMouse ? Color.Red : Color.White;
            ButtonSprite.Render();
            DisplayManager.DrawString(Font, Info.Text, TextPosition, TextColour);
        }

    }
}
