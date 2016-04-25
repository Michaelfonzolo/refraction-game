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
 * A form representing a button in the player's inventory.
 * 
 * Each button only stores information about the refractor type, not about the
 * quantity of said refractor the player has left in their inventory. These
 * quantities are recorded by the InventoryForm.
 */

#endregion

#region Using Statements

using DemeterEngine;
using DemeterEngine.Collision;
using DemeterEngine.Graphics;
using DemeterEngine.Multiforms.Forms;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

#endregion

namespace Refraction_V2.Multiforms.Level
{
    public class InventoryButtonForm : ButtonForm
    {

        /// <summary>
        /// The type of tile this button represents.
        /// </summary>
        public TileType Type { get; private set; }

        /// <summary>
        /// The sprite for the tile this button represents.
        /// </summary>
        public Sprite Sprite { get; private set; }

        /// <summary>
        /// The sprite for the button itself.
        /// </summary>
        public Sprite ButtonSprite { get; private set; }

        /// <summary>
        /// The sprite to draw overtop the button when the mouse is hovering over it.
        /// </summary>
        public Sprite HoverSprite { get; private set; }

        /// <summary>
        /// The number of items of this button's type left in the player's inventory.
        /// </summary>
        public int Quantity { get; private set; }

        /// <summary>
        /// Whether or not the quantity has changed. Used to indicate when to start
        /// scaling the quantity text.
        /// </summary>
        private bool QuantityChanged = false;

        /// <summary>
        /// The font used to display the quantity.
        /// </summary>
        public SpriteFont QuantityFont { get; private set; }

        /// <summary>
        /// The position of the quantity text.
        /// </summary>
        public Vector2 QuantityTextPosition { get; private set; }

        /// <summary>
        /// The amount by which we offset the quantity text position.
        /// </summary>
        private static readonly Vector2 QUANTITY_TEXT_POS_OFFSET = new Vector2(28, 0);

        /// <summary>
        /// Information regarding the fading in and out of the hover sprite.
        /// </summary>
        #region Hover Sprite Alpha Properties

        private float HoverSpriteAlpha = 0f;

        private const float HOVER_ALPHA_INCREMENT = 0.1f;

        private const float HOVER_ALPHA_MIN = 0f;

        private const float HOVER_ALPHA_MAX = 1f;

        private const int HOVER_ALPHA_MULTIPLIER = 130;

        #endregion

        /// <summary>
        /// Information regarding the scaling of the quantity text when the quantity changes.
        /// </summary>
        #region Quantity Text Scale Function Properties

        private float QuantityTextScale = QUANTITY_TEXT_SCALE_DEFAULT;

        private float QuantityTextScaleTime = 0f;

        private static readonly float QUANTITY_TEXT_SCALE_TIME_MAX = QTS_X1 + QTS_X2;

        private const float QUANTITY_TEXT_SCALE_TIME_INCREMENT = 0.03f;

        private const float QUANTITY_TEXT_SCALE_DEFAULT = 0.6f;

        /// <summary>
        /// The initial y-value of the text scale function.
        /// </summary>
        private const float QTS_Y0 = QUANTITY_TEXT_SCALE_DEFAULT;

        /// <summary>
        /// The maximum y-value of the text scale function.
        /// </summary>
        private const float QTS_Y1 = 1.1f;

        /// <summary>
        /// The initial x-value of the text scale function.
        /// </summary>
        private const float QTS_X0 = 0f;

        /// <summary>
        /// The x-value midway between the two piecewise components of the text scale function.
        /// </summary>
        private const float QTS_X1 = 0.1f;

        /// <summary>
        /// The maximum x-value of the text scale function.
        /// </summary>
        private const float QTS_X2 = 1.2f;

        /// <summary>
        /// The exponent in the text scale function's auxiliary function.
        /// </summary>
        private const float QTS_B = 3f;

        /// <summary>
        /// Auxiliary function e^(-|x|^b) used in the second piecewise component
        /// of the text scale function.
        /// </summary>
        private static float Phi(float time, float b)
        {
            return (float)Math.Exp(-Math.Pow(Math.Abs(time), b));
        }

        /// <summary>
        /// This is the function that controls how the quantity text scales when
        /// the quantity changes. It is a C1-continuous piecewise function constructed
        /// by patching together sin^2(x) and e^(-|x|^b)*cos^2(x).
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private float TextScaleFunction(float time)
        {
            var x1 = (float) Math.PI / 2 * (time - QTS_X0) / (QTS_X1 - QTS_X0);
            var x2 = (float) Math.PI / 2 * (time - QTS_X1) / (QTS_X2 - QTS_X1);
            if (QTS_X0 <= time && time < QTS_X1)
            {
                return QTS_Y0 + (QTS_Y1 - QTS_Y0) * (float)Math.Pow(Math.Sin(x1), 2.0);
            }
            else if (QTS_X1 <= time && time < QTS_X2)
            {
                return QTS_Y0 + (QTS_Y1 - QTS_Y0) * Phi(x2, QTS_B);
            }
            return QTS_Y0;
        }

        #endregion

        public InventoryButtonForm(TileType type, Vector2 topLeft, int quantity)
            : base(null, true)
        {
            Type = type;
            Sprite = new Sprite(GetTileTexture(type));
            ButtonSprite = new Sprite(Assets.Level.Images.InventoryButton);
            HoverSprite  = new Sprite(Assets.Level.Images.InventoryButtonHover);

            Collider = new RectCollider(topLeft, ButtonSprite.Width, ButtonSprite.Height);

            Quantity = quantity;
            QuantityFont = Assets.Level.Fonts.InventoryItem;
            QuantityTextPosition = ((RectCollider)Collider).Center + QUANTITY_TEXT_POS_OFFSET;
        }

        private Texture2D GetTileTexture(TileType type)
        {
            switch (type)
            {
                case TileType.RF_UxL_UL:
                    return Assets.Level.Images.Refractor_UxL_UL;
                case TileType.RF_UxR_UR:
                    return Assets.Level.Images.Refractor_UxR_UR;
                case TileType.RF_DxR_DR:
                    return Assets.Level.Images.Refractor_DxR_DR;
                case TileType.RF_DxL_DL:
                    return Assets.Level.Images.Refractor_DxL_DL;
                case TileType.RF_ULxUR_U:
                    return Assets.Level.Images.Refractor_ULxUR_U;
                case TileType.RF_DLxDR_D:
                    return Assets.Level.Images.Refractor_DLxDR_D;
                case TileType.RF_ULxDL_L:
                    return Assets.Level.Images.Refractor_ULxDL_L;
                case TileType.RF_URxDR_R:
                    return Assets.Level.Images.Refractor_URxDR_R;
                case TileType.RF_U_L_and_U_R_pass_UL:
                    return Assets.Level.Images.Refractor_U_L_and_U_R_pass_UL;
                case TileType.RF_U_L_and_U_R_pass_UR:
                    return Assets.Level.Images.Refractor_U_L_and_U_R_pass_UR;
                case TileType.RF_UL_DL_and_UR_DR_pass_L:
                    return Assets.Level.Images.Refractor_UL_DL_and_UR_DR_pass_L;
                case TileType.RF_UL_UR_and_DL_DR_pass_U:
                    return Assets.Level.Images.Refractor_UL_UR_and_DL_DR_pass_U;
                default:
                    throw new ArgumentException("Invalid TileType.");
            }
        }

        public override void Update()
        {
            base.Update();

            UpdateHoverSpriteAlpha();

            UpdateQuantityTextScale();
        }

        private void UpdateHoverSpriteAlpha()
        {
            var increment = (CollidingWithMouse ? 1 : -1) * HOVER_ALPHA_INCREMENT;
            HoverSpriteAlpha = MathHelper.Clamp(
                HoverSpriteAlpha + increment,
                HOVER_ALPHA_MIN,
                HOVER_ALPHA_MAX);
        }

        /// <summary>
        /// Update the scale of the quantity text.
        /// </summary>
        private void UpdateQuantityTextScale()
        {
            if (QuantityTextScaleTime > 0)
            {
                QuantityTextScale = TextScaleFunction(QuantityTextScaleTime);
                QuantityTextScaleTime += QUANTITY_TEXT_SCALE_TIME_INCREMENT;

                if (QuantityTextScaleTime >= QUANTITY_TEXT_SCALE_TIME_MAX)
                {
                    QuantityTextScaleTime = 0f;
                }
            }
            else
            {
                QuantityTextScale = QUANTITY_TEXT_SCALE_DEFAULT;
            }
            if (QuantityChanged)
            {
                QuantityChanged = false;
                QuantityTextScaleTime = QUANTITY_TEXT_SCALE_TIME_INCREMENT;
            }
        }

        /// <summary>
        /// Increment the quantity of this button's tile type.
        /// </summary>
        public void IncrementQuantity()
        {
            if (Quantity != LevelInfo.INFINITE_ITEMS_IN_INVENTORY)
            {
                Quantity++;
                QuantityChanged = true;
            }
        }

        /// <summary>
        /// Decrement the quantity of this button's tile type.
        /// </summary>
        public void DecrementQuantity()
        {
            if (Quantity != LevelInfo.INFINITE_ITEMS_IN_INVENTORY)
            {
                Quantity--;
                QuantityChanged = true;
            }
        }

        public override void Render()
        {
            var pos = ((RectCollider)Collider).TopLeft;

            ButtonSprite.Render(pos);
            Sprite.Render(pos);

            var text = Quantity == LevelInfo.INFINITE_ITEMS_IN_INVENTORY 
                                ? "INF" 
                                : String.Format("x{0}", Quantity);
            var textOffset = QuantityFont.MeasureString(text) / 2f;
            DisplayManager.DrawString(
                QuantityFont, text, QuantityTextPosition, Color.White, 0f, 
                textOffset, QuantityTextScale, SpriteEffects.None, 0f);
            
            if (HoverSpriteAlpha != 0)
            {
                DisplayManager.SetSpriteBatchProperties(blendState: BlendState.NonPremultiplied);

                HoverSprite.Alpha = HOVER_ALPHA_MULTIPLIER * HoverSpriteAlpha;
                HoverSprite.Render(pos);

                DisplayManager.ClearSpriteBatchProperties();
            }
        }
    }
}
