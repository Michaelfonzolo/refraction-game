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

using DemeterEngine;
using DemeterEngine.Collision;
using DemeterEngine.Graphics;
using DemeterEngine.Input;
using DemeterEngine.Maths;
using DemeterEngine.Multiforms.Forms;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Refraction_V2.Utils;

#endregion

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
            TextColour = info.InitialTextColour.HasValue ? info.InitialTextColour.Value 
                                                        : DefaultTextColour;
        }

        public void SetAlpha(float alpha)
        {
            ButtonSprite.Alpha = alpha;
            TextColour = new Color(TextColour, alpha);
        }

        public Vector2 GetPosition(PositionType positionType = PositionType.TopLeft)
        {
            var collider = (RectCollider)Collider;
            return PositionConverter.ToType(collider.TopLeft, collider.W, collider.H, PositionType.TopLeft, positionType);
        }

        public void SetPosition(Vector2 vec, PositionType positionType)
        {
            var collider = (RectCollider)Collider;
            var prevPosition = collider.TopLeft;
            var topLeft = PositionConverter.ToType(
                vec, collider.W, collider.H, positionType, PositionType.TopLeft);
            collider.SetPosition(topLeft);

            var delta = topLeft - prevPosition;
            TextPosition += delta;
            ButtonSprite.Position += delta;
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
