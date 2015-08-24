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

        public InventoryButtonForm(TileType type, RectCollider collider)
            : base(collider, true)
        {
            Type = type;
            Sprite = new Sprite(GetSpriteName(type));
            ButtonSprite = new Sprite(Assets.Level.Images.InventoryButton);
            HoverSprite = new Sprite(Assets.Level.Images.InventoryButtonHover);
        }

        private Texture2D GetSpriteName(TileType type)
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
                default:
                    throw new ArgumentException("Invalid TileType.");
            }
        }

        public override void Render()
        {
            var pos = ((RectCollider)Collider).TopLeft;

            ButtonSprite.Render(pos);
            Sprite.Render(pos);
            if (CollidingWithMouse)
                HoverSprite.Render(pos);
        }
    }
}
