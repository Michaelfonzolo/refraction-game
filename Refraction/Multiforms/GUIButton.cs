using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DemeterEngine;
using DemeterEngine.Collision;
using DemeterEngine.Input;
using DemeterEngine.Graphics;
using DemeterEngine.Maths;
using DemeterEngine.Multiforms.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Refraction_V2.Utils;

namespace Refraction_V2.Multiforms
{
    enum GUIButton_ShiftDirection
    {
        Down,
        Up,
        None
    }

    public class GUIButton : ButtonForm, ITransitionalForm
    {

        /// <summary>
        /// The default colour used by GUIButtons.
        /// </summary>
        public static readonly Color DefaultTextColour = Color.Black;

        public static readonly Vector2 TextPositionOffset = new Vector2(0, 0);

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

        /// <summary>
        /// Information regarding how the gui button shifts down left as it gets pressed.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="position"></param>
        /// <param name="positionType"></param>
        #region Button On-Click Offset Info

        private GUIButton_ShiftDirection Shifting = GUIButton_ShiftDirection.None;

        /// <summary>
        /// The amount by which the button is shifted from it's initial position.
        /// </summary>
        private float ShiftAmount = SHIFT_AMOUNT_MIN;

        private const float SHIFT_AMOUNT_INCREMENT = 0.25f;

        private const float SHIFT_AMOUNT_MIN = 0f;

        private const float SHIFT_AMOUNT_MAX = 1f;

        private const float SHIFT_MULTIPLIER = 2.2f;

        private static readonly Vector2 SHIFT_DIRECTION = VectorUtils.GameWorldDown;

        private Vector2 Shift
        {
            get
            {
                return ShiftAmount * SHIFT_MULTIPLIER * SHIFT_DIRECTION;
            }
        }

        #endregion

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

        public void SetAlpha(float alpha)
        {
            ButtonSprite.Alpha = alpha;
            TextColour = new Color(TextColour, alpha);
        }

        private void UpdateShift()
        {
            if ((CollidingWithMouse && MouseInput.IsClicked(MouseButtons.Left)) ||
                (MouseEntered && MouseInput.IsPressed(MouseButtons.Left)))
            {
                Shifting = GUIButton_ShiftDirection.Up;
            }
            else if (MouseExited || MouseInput.IsReleased(MouseButtons.Left))
            {
                Shifting = GUIButton_ShiftDirection.Down;
            }

            if (Shifting != GUIButton_ShiftDirection.None)
            {
                var direction = Shifting == GUIButton_ShiftDirection.Up ? 1 : -1;
                ShiftAmount += direction * SHIFT_AMOUNT_INCREMENT;
            }

            if ((Shifting == GUIButton_ShiftDirection.Up && ShiftAmount >= SHIFT_AMOUNT_MAX) ||
                (Shifting == GUIButton_ShiftDirection.Down && ShiftAmount <= SHIFT_AMOUNT_MIN))
            {
                ShiftAmount = Shifting == GUIButton_ShiftDirection.Up ? SHIFT_AMOUNT_MAX : SHIFT_AMOUNT_MIN;
                Shifting = GUIButton_ShiftDirection.None;
            }
        }

        public override void Update()
        {
            base.Update();

            UpdateShift();
        }

        public override void Render()
        {
            ButtonSprite.TintRGB = CollidingWithMouse ? Color.Red : Color.White;

            DisplayManager.SetSpriteBatchProperties(blendState: BlendState.NonPremultiplied);

            ButtonSprite.Render(ButtonSprite.Position + Shift, true);
            DisplayManager.DrawString(Font, Info.Text, TextPosition + Shift, TextColour);

            DisplayManager.ClearSpriteBatchProperties();
        }

    }
}
